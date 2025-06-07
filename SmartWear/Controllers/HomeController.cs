using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SmartWear.Models;

namespace SmartWear.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult Index() => View();
        public IActionResult About() => View();
        public IActionResult Contact() => View();
        public IActionResult Privacy() => View();
        public IActionResult Support() => View();
        public IActionResult FAQ() => View();
        public IActionResult TOS() => View();
        public IActionResult ReturnPolicy() => View();
        public IActionResult StarterPage() => View();
        public IActionResult NotFound404() => View("404");

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
