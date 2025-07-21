using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using SmartWear.Models;
using SmartWear.ViewModels;
using System.Diagnostics;

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
        public IActionResult SearchResults() => View();
        public async Task<IActionResult> Category(string categoryName,int page = 1)
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

        public async Task<IActionResult> ProductDetails(Guid id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return View(product);
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
