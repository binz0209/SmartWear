using Microsoft.AspNetCore.Mvc;
using Business.Data;
using System.Linq;

namespace SmartWearAdmin.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _context.Users
                .Where(u => u.Username == username && u.PasswordHash == password && !u.IsDeleted)
                .Select(u => new { u.Id, u.Username, RoleName = u.Role.Name })
                .FirstOrDefault();

            if (user == null || user.RoleName != "Admin")
            {
                ViewBag.Error = "Sai tài khoản hoặc mật khẩu";
                return View();
            }

            HttpContext.Session.SetString("AdminId", user.Id.ToString());
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
