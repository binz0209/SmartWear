using System.Diagnostics;
using System.Security.Claims;
using Business.Models;
using Business.Models.vnPay;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services;
using Services.VnPay;
using SmartWear.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace SmartWear.Controllers
{
    public class CartController : Controller
    {
        private readonly ILogger<CartController> _logger;
        private readonly ICartService _cartService;
        private readonly IVnPayService _vnPayService;
        private readonly IAddressService _addressService;
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;

        public CartController(ILogger<CartController> logger, ICartService cartService, IVnPayService vnPayService,
            IAddressService addressService, IPaymentService paymentService, IOrderService orderService)
        {
            _logger = logger;
            _cartService = cartService;
            _vnPayService = vnPayService;
            _addressService = addressService;
            _paymentService = paymentService;
            _orderService = orderService;
        }

        public IActionResult Index() => View(); // Cart

        public async Task<IActionResult> Checkout()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return RedirectToAction("Login", "Account");

            var userId = Guid.Parse(userIdClaim.Value);
            var cart = await _cartService.GetCartWithItemsByUserIdAsync(userId);

            if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
                return RedirectToAction("Index", "Cart");

            return View(cart.CartItems);
        }

        public IActionResult ShippingInfo() => View();
        public IActionResult PaymentMethods() => View();

        public async Task<IActionResult> OrderConfirmation(Guid orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null) return RedirectToAction("Index");

            return View(order);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> CreatePaymentUrlVnpay(PaymentInformationModel model)
        {
            _logger.LogInformation("Creating payment URL for VnPay with model: {@Model}", model);

            // Lưu vào Session
            HttpContext.Session.SetString("Address.FullName", Request.Form["Name"]);
            HttpContext.Session.SetString("Address.PhoneNumber", Request.Form["PhoneNumber"]);
            HttpContext.Session.SetString("Address.Email", Request.Form["Email"]);
            HttpContext.Session.SetString("Address.StreetAddress", Request.Form["StreetAddress"]);
            HttpContext.Session.SetString("Address.City", Request.Form["City"]);

            // Kiểm tra thiếu dữ liệu
            if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("Address.FullName")) ||
                string.IsNullOrWhiteSpace(HttpContext.Session.GetString("Address.PhoneNumber")) ||
                string.IsNullOrWhiteSpace(HttpContext.Session.GetString("Address.StreetAddress")) ||
                string.IsNullOrWhiteSpace(HttpContext.Session.GetString("Address.City")))
            {
                TempData["Error"] = "Vui lòng nhập đầy đủ thông tin giao hàng.";
                return RedirectToAction("Checkout");
            }

            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);
            return Redirect(url);
        }

        [HttpGet]
        public async Task<IActionResult> PaymentCallbackVnpay()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            _logger.LogInformation("\n\nVNPAY response JSON: {ResponseJson}", System.Text.Json.JsonSerializer.Serialize(response));
            if (!response.Success || response.VnPayResponseCode != "00")
                return RedirectToAction("Checkout");


            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return RedirectToAction("Login", "Account");
            var userId = Guid.Parse(userIdClaim.Value);

            var cart = await _cartService.GetCartWithItemsByUserIdAsync(userId);
            if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
                return RedirectToAction("Index");

            // Lấy địa chỉ từ Session
            var address = new Address
            {
                UserId = userId,
                FullName = HttpContext.Session.GetString("Address.FullName"),
                PhoneNumber = HttpContext.Session.GetString("Address.PhoneNumber"),
                StreetAddress = HttpContext.Session.GetString("Address.StreetAddress"),
                City = HttpContext.Session.GetString("Address.City")
            };
            await _addressService.AddAddressAsync(address);

            var order = new Order
            {
                UserId = userId,
                AddressId = address.Id,
                OrderDate = DateTime.UtcNow,
                Status = "Confirmed",
                OrderItems = cart.CartItems.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.Price
                }).ToList()
            };
            await _orderService.AddOrderAsync(order);

            var payment = new Payment
            {
                OrderId = order.Id,
                IsPaid = true,
                PaymentDate = DateTime.UtcNow,
                PaymentMethod = response.PaymentMethod
            };
            await _paymentService.AddPaymentAsync(payment);

            await _cartService.ClearCartAsync(cart.Id);

            // Xóa session sau khi dùng
            HttpContext.Session.Remove("Address.FullName");
            HttpContext.Session.Remove("Address.PhoneNumber");
            HttpContext.Session.Remove("Address.StreetAddress");
            HttpContext.Session.Remove("Address.City");

            return RedirectToAction("OrderConfirmation", new { orderId = order.Id });
        }
    }
}
