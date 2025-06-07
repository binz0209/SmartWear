using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SmartWear.Models;

namespace SmartWear.Controllers
{
    public class CartController : Controller
    {
        private readonly ILogger<CartController> _logger;

        public CartController(ILogger<CartController> logger)
        {
            _logger = logger;
        }
        public IActionResult Index() => View(); // Cart
        public IActionResult Checkout() => View();
        public IActionResult ShippingInfo() => View();
        public IActionResult PaymentMethods() => View();
        public IActionResult OrderConfirmation() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
