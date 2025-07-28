using Business.Data;
using Microsoft.AspNetCore.Mvc;

namespace SmartWearAdmin.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StatisticsController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult GetCategories()
        {
            // Trả về danh mục dưới dạng JSON
            var categories = _context.Categories
                .Select(c => new { c.Id, c.Name })
                .ToList();

            return Json(categories);
        }

        [HttpGet]
        public IActionResult GetProductsByCategory(Guid categoryId)
        {
            var data = _context.Products
                .Where(p => p.CategoryId == categoryId)
                .Select(p => new
                {
                    ProductName = p.Name,
                    StockQuantity = p.StockQuantity
                }).ToList();

            return Json(data);
        }
    }
}
