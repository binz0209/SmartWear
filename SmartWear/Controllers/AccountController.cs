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
        private readonly IOrderService _orderService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IUserService userService, IRoleService roleService, IOrderService orderService, ILogger<AccountController> logger)
        {
            _userService = userService;
            _roleService = roleService;
            _orderService = orderService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Account()
        {
            // Lấy userId từ claims
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdValue))
                return RedirectToAction("Login");

            var userId = Guid.Parse(userIdValue);

            // Lấy thông tin user
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
                return RedirectToAction("Login");

            // Lấy tất cả order, bao gồm luôn OrderItems → Product, Payment, Address
            var allOrders = await _orderService.GetAllOrdersAsync();

            // Lọc ra chỉ các order của user này, và map vào OrderViewModel
            var userOrders = allOrders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .Select(o =>
                {
                    // Tính subtotal
                    var subtotal = o.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity);

                    // Giả sử phí ship cố định hoặc do bạn tự tính
                    decimal shipping = 0m;
                    // Giả sử tax = 10% subtotal
                    decimal tax = Math.Round(subtotal * 0.1m, 2);

                    return new OrderViewModel
                    {
                        Id = o.Id,
                        OrderDate = o.OrderDate,
                        // giữ raw code nếu cần dùng cho CSS class
                        Status = o.Status,
                        // text-friendly status
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
                                                       SKU = oi.Product.Id.ToString()
                                                                         .Substring(0, 8)
                                                                         .ToUpper(),
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
            _logger.LogInformation("UpdateProfile action triggered");

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogWarning("Model error: " + error.ErrorMessage);
                }
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
    }
}
