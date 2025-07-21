using System.Diagnostics;
using Business.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Services;
using SmartWear.Models;
using SmartWear.ViewModels;

namespace SmartWear.Controllers
{
    public class CartController : Controller
    {
        private readonly ILogger<CartController> _logger;
        private readonly ICartService _cartService;
        private readonly ICartItemService _cartItemService;
        public CartController(ILogger<CartController> logger, ICartService cartService, ICartItemService cartItemService)
        {
            _logger = logger;
            _cartService = cartService;
            _cartItemService = cartItemService;
        }
        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return RedirectToAction("Login", "Account");

            var userId = Guid.Parse(userIdClaim.Value);
            var cart = await _cartService.GetCartWithItemsByUserIdAsync(userId);

            return View(cart?.CartItems ?? new List<CartItem>());
        }
        public IActionResult Checkout() => View();
        public IActionResult ShippingInfo() => View();
        public IActionResult PaymentMethods() => View();
        public IActionResult OrderConfirmation() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpPost]
        public async Task<IActionResult> AddToCart(Guid productId, int quantity = 1)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return RedirectToAction("Login", "Account");

            var userId = Guid.Parse(userIdClaim.Value);

            await _cartService.AddToCartAsync(userId, productId, quantity);

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateQuantities(List<UpdateCartItemViewModel> items)
        {
            foreach (var item in items)
            {
                await _cartItemService.UpdateQuantityAsync(item.CartItemId, item.Quantity);
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> RemoveItem(Guid cartItemId)
        {
            await _cartItemService.DeleteCartItemAsync(cartItemId);
            return RedirectToAction("Index");
        }
    }
}
