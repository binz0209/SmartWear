using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Services;
using SmartWear.Models;
using SmartWear.ViewModels;

namespace SmartWear.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductService _productService;

        public ProductController(ILogger<ProductController> logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }
        public async Task<IActionResult> SearchResults(string keyword, string sortOrder, int page = 1)
        {
            int pageSize = 8;
            var products = await _productService.SearchProductsAsync(keyword);

            // Sort logic
            products = sortOrder switch
            {
                "date-desc" => products.OrderByDescending(p => p.CreatedOn),
                "date-asc" => products.OrderBy(p => p.CreatedOn),
                "title-asc" => products.OrderBy(p => p.Name),
                "title-desc" => products.OrderByDescending(p => p.Name),
                _ => products
            };

            var totalItems = products.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var pagedProducts = products
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    Rating = p.ProductReviews.Any() ? (int)Math.Round(p.ProductReviews.Average(r => r.Rating)) : 0,
                    ReviewCount = p.ProductReviews.Count
                }).ToList();

            var model = new SearchViewModel
            {
                Keyword = keyword,
                Products = pagedProducts,
                CurrentPage = page,
                TotalPages = totalPages,
                SortOrder = sortOrder
            };

            return View(model);
        }
        public IActionResult Category(string categoryName) => View();
        public async Task<IActionResult> ProductDetails(Guid id)
        {   
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();

            var viewModel = new ProductDetailsViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                CategoryName = product.Category?.Name,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                Color = product.Color,
                Size = product.Size,
                ImageUrl = product.ImageUrl,
                Rating = product.ProductReviews != null && product.ProductReviews.Any() ? Math.Round(product.ProductReviews.Average(r => r.Rating), 1) : 0,
                ReviewCount = product.ProductReviews?.Count ?? 0,
            };

            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
