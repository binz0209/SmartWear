using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SmartWear.Models;

namespace SmartWear.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;

        public ProductController(ILogger<ProductController> logger)
        {
            _logger = logger;
        }
        public IActionResult SearchResults() => View();
        public IActionResult Category(string categoryName) => View();
        public IActionResult ProductDetails(int id) => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
