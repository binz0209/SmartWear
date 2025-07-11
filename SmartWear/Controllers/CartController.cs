using System.Diagnostics;
using System.Security.Claims;
using Business.Models.vnPay;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services;
using Services.VnPay;
using SmartWear.Models;

namespace SmartWear.Controllers
{
    public class CartController : Controller
    {
        private readonly ILogger<CartController> _logger;
        private readonly ICartService _cartService;
        private readonly IVnPayService _vnPayService;

        public CartController(ILogger<CartController> logger, ICartService cartService, IVnPayService vnPayService)
        {
            _logger = logger;
            _cartService = cartService;
            _vnPayService = vnPayService;
        }
        public IActionResult Index() => View(); // Cart

        public async Task<IActionResult> Checkout()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            var userId = Guid.Parse(userIdClaim.Value);
            var cart = await _cartService.GetCartWithItemsByUserIdAsync(userId);

            if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
                return RedirectToAction("Index", "Cart"); 

            return View(cart.CartItems);
        }


        public IActionResult ShippingInfo() => View();
        public IActionResult PaymentMethods() => View();
        public IActionResult OrderConfirmation() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpPost]
        public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel model)
        {
            // Debug model
            _logger.LogInformation("Creating payment URL for VnPay with model: {@Model}", model);
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

            return Redirect(url);
        }

        [HttpGet]
        public IActionResult PaymentCallbackVnpay()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            return Json(response);
        }

    }
}
