using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SmartWear.Models;

namespace SmartWear.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;

        public AccountController(ILogger<AccountController> logger)
        {
            _logger = logger;
        }
        public IActionResult Account() => View();
        public IActionResult Login() => View();
        public IActionResult Register() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
