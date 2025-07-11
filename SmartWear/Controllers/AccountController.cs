﻿using BCrypt.Net;
using Business.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Services;
using SmartWear.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace SmartWear.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IUserService userService, IRoleService roleService, ILogger<AccountController> logger)
        {
            _userService = userService;
            _roleService = roleService;
            _logger = logger;
        }
        public IActionResult Account() => View();
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
