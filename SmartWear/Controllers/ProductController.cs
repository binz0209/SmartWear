using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using SmartWear.Models;
using SmartWear.ViewModels;
using System.Diagnostics;
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

        // SearchResults - từ ProductAndCart
        public async Task<IActionResult> SearchResults(string keyword, string sortOrder, int page = 1)
        {
            int pageSize = 8;
            var products = await _productService.SearchProductsAsync(keyword);

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

        // Category với pagination - giữ từ HEAD
        public async Task<IActionResult> Category(string categoryName, int page = 1)
        {
            int pageSize = 5;
            var allProducts = await _productService.GetAllProductsAsync();

            var filtered = string.IsNullOrEmpty(categoryName)
                ? allProducts
                : allProducts.Where(p => p.Category != null &&
                                         p.Category.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));

            int totalProducts = filtered.Count();
            var pagedProducts = filtered
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var viewModel = new CategoryProductListViewModel
            {
                SelectedCategory = categoryName,
                Products = pagedProducts.Select(p => new ProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    Size = p.Size,
                    Color = p.Color,
                    CategoryName = p.Category?.Name
                }).ToList(),
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalProducts / pageSize),
                PageSize = pageSize
            };

            return View(viewModel);
        }

        // ProductDetails - nên dùng bản từ ProductAndCart
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

        [HttpGet]
        public async Task<IActionResult> FilterByCategory(string categoryName)
        {
            var allProducts = await _productService.GetAllProductsAsync();

            var filtered = string.IsNullOrEmpty(categoryName)
                ? allProducts
                : allProducts.Where(p => p.Category != null &&
                                         p.Category.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));

            var productsVm = filtered.Select(p => new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                Size = p.Size,
                Color = p.Color,
                CategoryName = p.Category?.Name
            }).ToList();

            return PartialView("_ProductListPartial", productsVm);
        }

        [HttpGet]
        public async Task<IActionResult> FilterByPrice(decimal minPrice, decimal maxPrice)
        {
            var allProducts = await _productService.GetAllProductsAsync();

            var filtered = allProducts
                .Where(p => p.Price >= minPrice && p.Price <= maxPrice);

            var productsVm = filtered.Select(p => new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                Size = p.Size,
                Color = p.Color,
                CategoryName = p.Category?.Name
            }).ToList();

            return PartialView("_ProductListPartial", productsVm);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string keyword)
        {
            var allProducts = await _productService.GetAllProductsAsync();

            var filtered = allProducts
                .Where(p => !string.IsNullOrEmpty(p.Name) && p.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                            !string.IsNullOrEmpty(p.Description) && p.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase));

            var productsVm = filtered.Select(p => new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                Color = p.Color,
                Size = p.Size
            }).ToList();

            return PartialView("_ProductListPartial", productsVm);
        }

        [HttpGet]
        public async Task<IActionResult> FilterByColor([FromQuery] List<string> colors)
        {
            var allProducts = await _productService.GetAllProductsAsync();

            var filtered = colors == null || !colors.Any()
                ? allProducts
                : allProducts.Where(p => !string.IsNullOrEmpty(p.Color) && colors.Contains(p.Color.ToLower()));

            var productsVm = filtered.Select(p => new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                Size = p.Size,
                Color = p.Color,
                CategoryName = p.Category?.Name
            }).ToList();

            return PartialView("_ProductListPartial", productsVm);
        }

        [HttpGet]
        public async Task<IActionResult> FilterByCategoryWithPagination(string categoryName, int page = 1)
        {
            int pageSize = 5;
            var allProducts = await _productService.GetAllProductsAsync();

            var filtered = string.IsNullOrEmpty(categoryName)
                ? allProducts
                : allProducts.Where(p => p.Category != null &&
                                         p.Category.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));

            int totalProducts = filtered.Count();
            var pagedProducts = filtered
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var viewModel = new CategoryProductListViewModel
            {
                SelectedCategory = categoryName,
                Products = pagedProducts.Select(p => new ProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    Size = p.Size,
                    Color = p.Color,
                    CategoryName = p.Category?.Name
                }).ToList(),
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalProducts / pageSize),
                PageSize = pageSize
            };

            return PartialView("_ProductListWithPaginationPartial", viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

}
