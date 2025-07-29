using BCrypt.Net;
using Business.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using SmartWear.Models;
using SmartWear.ViewModels;
using System.Diagnostics;
using System.Security.Claims;

namespace SmartWear.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IEmailOtpService _otpService;
        private readonly ILogger<AccountController> _logger;
        private readonly IOrderService _orderService;
        private readonly IAuditLogService _auditLogService;

        public AccountController(IUserService userService, IRoleService roleService, ILogger<AccountController> logger,
            IEmailOtpService otpService, IOrderService orderService, IAuditLogService auditLogService)
        {
            _userService = userService;
            _roleService = roleService;
            _orderService = orderService;
            _logger = logger;
            _otpService = otpService;
            _auditLogService = auditLogService;
        }

        [HttpGet]
        public async Task<IActionResult> Account()
        {
            // Lấy userId từ claims
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdValue))
                return RedirectToAction("Login");

            var userId = Guid.Parse(userIdValue);
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
                return RedirectToAction("Login");

            ViewBag.ActiveTab = TempData["ActiveTab"]?.ToString() ?? "orders";

            var allOrders = await _orderService.GetAllOrdersAsync();

            var userOrders = allOrders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .Select(o =>
                {
                    var subtotal = o.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity);
                    decimal shipping = 0m;
                    decimal tax = Math.Round(subtotal * 0.1m, 2);

                    return new OrderViewModel
                    {
                        Id = o.Id,
                        OrderDate = o.OrderDate,
                        Status = o.Status,
                        StatusDisplay = o.Status switch
                        {
                            "0" => "In progress",
                            "1" => "Delivered",
                            "2" => "Canceled",
                            _ => o.Status
                        },
                        Subtotal = subtotal,
                        Shipping = shipping,
                        Tax = tax,
                        Total = subtotal + shipping + tax,
                        PaymentMethodDisplay = o.Payment?.PaymentMethod ?? "Chưa thanh toán",
                        ProductThumbnails = o.OrderItems
                                              .Select(oi => oi.Product.ImageUrl ?? "/img/placeholder.png")
                                              .Take(3)
                                              .ToList(),
                        Items = o.OrderItems
                                 .Select(oi => new OrderItemViewModel
                                 {
                                     ProductName = oi.Product.Name,
                                     SKU = oi.Product.Id.ToString().Substring(0, 8).ToUpper(),
                                     Quantity = oi.Quantity,
                                     UnitPrice = oi.UnitPrice
                                 })
                                 .ToList(),
                        ShippingAddress = $"{o.Address.StreetAddress}, {o.Address.City}",
                        ShippingMethod = "Standard Shipping"
                    };
                })
                .ToList();

            var model = new UserProfileViewModel
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Orders = userOrders
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(UserProfileViewModel model)
        {
            TempData["ActiveTab"] = "personal";

            if (!ModelState.IsValid)
            {
                return View("Account", model);
            }

            var user = await _userService.GetUserByIdAsync(model.Id);
            if (user == null)
            {
                TempData["Error"] = "Không tìm thấy người dùng.";
                return RedirectToAction("Account");
            }

            user.Username = model.Username;
            user.Email = model.Email;
            await _userService.UpdateUserAsync(user);

            TempData["Success"] = "Thông tin đã được cập nhật.";
            return RedirectToAction("Account");
        }

        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginPost()
        {
            string email = Request.Form["username"].ToString().Trim().ToLower();
            string password = Request.Form["password"];

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin.";
                return View("Login");
            }

            // Tìm user
            var user = (await _userService.GetAllUsersAsync())
                       .FirstOrDefault(u => u.Username.ToLower() == email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                ViewBag.Error = "Email hoặc mật khẩu không chính xác.";
                return View("Login");
            }

            // Tạo Claims
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role?.Name ?? "Customer")
    };

            var claimsIdentity = new ClaimsIdentity(claims, "MyCookieAuth");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
            };

            await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal, authProperties);

            // AUDIT LOG: Đăng nhập thành công
            await AddAuditLogAsync("Login", $"User {user.Username} logged in.", user.Id);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterPost()
        {
            // Lấy dữ liệu từ form
            string firstName = Request.Form["firstName"];
            string lastName = Request.Form["lastName"];
            string email = Request.Form["email"];
            string password = Request.Form["password"];
            string confirmPassword = Request.Form["confirmPassword"];

            // Sử dụng email làm username
            string username = email.Trim().ToLower();

            // Validate cơ bản
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin.";
                return View("Register");
            }

            if (password != confirmPassword)
            {
                ViewBag.Error = "Mật khẩu xác nhận không đúng.";
                return View("Register");
            }

            // Kiểm tra trùng username/email
            var existingUsers = await _userService.GetAllUsersAsync();
            if (existingUsers.Any(u => u.Username == username))
            {
                ViewBag.Error = "Email này đã được dùng làm tài khoản.";
                return View("Register");
            }

            if (existingUsers.Any(u => u.Email == email))
            {
                ViewBag.Error = "Email đã được sử dụng.";
                return View("Register");
            }

            // Lấy role mặc định
            var role = (await _roleService.GetAllRolesAsync()).FirstOrDefault(r => r.Name == "Customer");
            if (role == null)
            {
                ViewBag.Error = "Không tìm thấy vai trò mặc định.";
                return View("Register");
            }

            // Hash mật khẩu
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Username = username,         
                Email = email,
                PasswordHash = hashedPassword,
                RoleId = role.Id,
                CreatedOn = DateTime.UtcNow,
                IsDeleted = false
            };

            await _userService.AddUserAsync(newUser);

            // Đăng ký thành công → chuyển đến login
            return RedirectToAction("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Lấy userId từ Claims
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdClaim, out var userId);

            await AddAuditLogAsync("Logout", "User logged out.", userId);

            await HttpContext.SignOutAsync("MyCookieAuth");
            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(Guid id)
        {
            // 1) Lấy userId hiện tại
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdValue, out var userId))
                return Forbid();

            // 2) Lấy đơn từ DB (khi này EF Context đã track order)
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null || order.UserId != userId)
                return NotFound();

            // 3) Chỉ hủy khi đang 'In progress' (mã "0")
            if (order.Status == "0")
            {
                order.Status = "2";            // 2 = Canceled
                await _orderService.UpdateOrderAsync(order);
            }

            // 4) Redirect về lại trang Account
            return RedirectToAction(nameof(Account));
        }


        [HttpGet]
        public IActionResult GoogleLogin()
        {
            var redirectUrl = Url.Action("GoogleLoginCallback", "Account"); // callback sau khi xác thực
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, "Google");
        }

        public async Task<IActionResult> GoogleLoginCallback()
        {
            // Google middleware sẽ tự xử lý đăng nhập và gán vào User
            var externalUser = User;

            if (externalUser == null || !externalUser.Identity.IsAuthenticated)
                return RedirectToAction("Login");

            var email = externalUser.FindFirst(ClaimTypes.Email)?.Value;
            var name = externalUser.FindFirst(ClaimTypes.Name)?.Value;

            // Tìm hoặc tạo user như bạn đã làm
            var existingUsers = await _userService.GetAllUsersAsync();
            var user = existingUsers.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                var role = (await _roleService.GetAllRolesAsync()).FirstOrDefault(r => r.Name == "Customer");

                user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = email,
                    Username = email,
                    PasswordHash = "", // vì dùng google
                    RoleId = role?.Id ?? Guid.NewGuid(),
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false
                };

                await _userService.AddUserAsync(user);
            }

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role?.Name ?? "Customer")
    };

            var claimsIdentity = new ClaimsIdentity(claims, "MyCookieAuth");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(UserProfileViewModel model)
        {
            TempData["ActiveTab"] = "password";


            if (string.IsNullOrWhiteSpace(model.CurrentPassword) ||
                string.IsNullOrWhiteSpace(model.NewPassword) ||
                string.IsNullOrWhiteSpace(model.ConfirmNewPassword))
            {
                TempData["Error"] = "Vui lòng nhập đầy đủ thông tin.";
                return RedirectToAction("Account");
            }

            if (model.NewPassword != model.ConfirmNewPassword)
            {
                TempData["Error"] = "Mật khẩu mới không khớp.";
                return RedirectToAction("Account");
            }

            var user = await _userService.GetUserByIdAsync(model.Id);
            if (user == null)
            {
                TempData["Error"] = "Không tìm thấy người dùng.";
                return RedirectToAction("Account");
            }

            if (string.IsNullOrEmpty(user.PasswordHash) || !BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.PasswordHash))
            {
                TempData["Error"] = "Mật khẩu hiện tại không chính xác.";
                return RedirectToAction("Account");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            await _userService.UpdateUserAsync(user);

            TempData["Success"] = "Đổi mật khẩu thành công.";
            return RedirectToAction("Account");
        }

        // GET: /Account/ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword()
            => View(new ForgotPasswordViewModel());

        // POST: /Account/ForgotPassword
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var user = await _userService.GetByEmailAsync(vm.Email);
            if (user != null)
            {
                await _otpService.SendOtpAsync(vm.Email);
                TempData["Info"] = "Mã OTP đã được gửi vào email của bạn.";
                return RedirectToAction("ResetPassword", new { email = vm.Email });
            }

            ModelState.AddModelError("", "Không tìm thấy email trong hệ thống.");
            return View(vm);
        }

        // GET: /Account/ResetPassword?email=...
        [HttpGet]
        public IActionResult ResetPassword(string email)
            => View(new ResetPasswordViewModel { Email = email });

        // POST: /Account/ResetPassword
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            if (!await _otpService.ValidateOtpAsync(vm.Email, vm.OTP))
            {
                ModelState.AddModelError("OTP", "OTP không hợp lệ hoặc đã hết hạn.");
                return View(vm);
            }

            var user = await _userService.GetByEmailAsync(vm.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Không tìm thấy người dùng.");
                return View(vm);
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(vm.NewPassword);
            await _userService.UpdateUserAsync(user);

            TempData["Success"] = "Đổi mật khẩu thành công! Vui lòng đăng nhập lại.";
            return RedirectToAction("Login");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task AddAuditLogAsync(string action, string description, Guid userId)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var log = new AuditLog
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Action = action,
                Description = description,
                IpAddress = ip,
                CreatedOn = DateTime.UtcNow
            };
            await _auditLogService.AddAuditLogAsync(log);
        }

    }
}
