using Microsoft.AspNetCore.Mvc;
using Services;
using SmartWear.Models;
using SmartWear.ViewModels;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

public class ProductController : Controller
{
    private readonly ILogger<ProductController> _logger;
    private readonly ICategoryService _categoryService;
    private readonly IProductService _productService;

    public ProductController(ILogger<ProductController> logger, ICategoryService categoryService, IProductService productService)
    {
        _logger = logger;
        _categoryService = categoryService;
        _productService = productService;
    }

    public async Task<IActionResult> Category(Guid? categoryId)
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        var products = await _productService.GetAllProductsAsync();
        if (categoryId.HasValue)
        {
            products = products.Where(p => p.CategoryId == categoryId.Value).ToList();
        }
        var categoryViewModels = categories.Select(category => new CategoryProductViewModel
        {
            Id = category.Id,
            Name = category.Name,
            Products = products
                .Where(p => p.CategoryId == category.Id)  
                .Select(p => new ProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl
                }).ToList()
        }).ToList();

        bool hasProducts = categoryViewModels.Any(c => c.Products.Any());

        //No products
        ViewBag.NoProducts = hasProducts ? null : "There are no products available for this category.";

        return View(categoryViewModels);  
    }

    public IActionResult SearchResults() => View();

    public IActionResult ProductDetails(int id) => View();
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
