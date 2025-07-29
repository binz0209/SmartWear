using Microsoft.AspNetCore.Mvc;
using Business.Data;
using System.Linq;
using System;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace SmartWearAdmin.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        private async Task AddAuditLog(string action, string description, Guid userId)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

            await _context.AuditLogs.AddAsync(new Business.Models.AuditLog
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Action = action,
                Description = description,
                IpAddress = ip,
                CreatedOn = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = _context.Users
                .Where(u => u.Username == username && u.PasswordHash == password && !u.IsDeleted)
                .Select(u => new { u.Id, u.Username, RoleName = u.Role.Name })
                .FirstOrDefault();

            if (user == null || user.RoleName != "Admin")
            {
                // Optional: Audit failed logins
                // await AddAuditLog("LoginFailed", $"Failed login with username: {username}", Guid.Empty);

                ViewBag.Error = "Incorrect username or password";
                return View();
            }

            HttpContext.Session.SetString("AdminId", user.Id.ToString());

            await AddAuditLog("Login", $"Admin: {user.Username} logged in.", user.Id);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            Guid userId = Guid.Empty;
            if (Guid.TryParse(HttpContext.Session.GetString("AdminId"), out Guid uid)) userId = uid;

            await AddAuditLog("Logout", $"Admin logged out.", userId);

            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
