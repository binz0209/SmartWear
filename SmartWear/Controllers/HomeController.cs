using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Services;
using SmartWear.Models;
using SmartWear.ViewModels;

namespace SmartWear.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;

        public HomeController(ILogger<HomeController> logger , IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }
        public async Task<IActionResult> Index()
        {
            var allProducts = (await _productService.GetAllProductsAsync()).ToList();

            List<ProductViewModel> MapToVM(IEnumerable<Business.Models.Product> products) =>
        products.Select(p => new ProductViewModel
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            ImageUrl = p.ImageUrl,
            Rating = p.ProductReviews.Any() ? (int)Math.Round(p.ProductReviews.Average(r => r.Rating)) : 0,
            ReviewCount = p.ProductReviews.Count
        }).ToList();

            var viewModel = new HomeViewModel
            {
                NewArrivals = MapToVM(allProducts.Where(x => x.Category != null && x.Category.Name == "New Arrivals").Take(4)),
                SaleProducts = MapToVM(allProducts.Where(x => x.Category != null && x.Category.Name == "Sale").Take(4)),
                FeaturedProducts = MapToVM(allProducts.Where(x => x.Category != null && x.Category.Name == "Featured").Take(4)),
                BestSellers = MapToVM(allProducts.Where(x => x.OrderItems != null && x.OrderItems.Count > 0).OrderByDescending(x => x.OrderItems?.Count ?? 0).Take(8)),
                AllProducts = MapToVM(allProducts.Take(8))
            };

            return View(viewModel);
        }
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
