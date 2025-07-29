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
        [HttpGet]
        public IActionResult GetOrderStatusCounts()
        {
            var data = _context.Orders
                .GroupBy(o => o.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                }).ToList();

            return Json(data);
        }
        [HttpGet]
        public IActionResult GetRegisterChartData(string period)
        {
            var query = _context.AuditLogs
                .Where(a => a.Action == "Register");

            DateTime now = DateTime.UtcNow;
            Dictionary<string, int> result = new();

            if (period == "week")
            {
                var startOfWeek = now.Date.AddDays(-(int)now.DayOfWeek + 1);
                for (int i = 0; i < 7; i++)
                {
                    var day = startOfWeek.AddDays(i);
                    string label = day.ToString("dddd", new System.Globalization.CultureInfo("en-US"));
                    int count = query.Count(a => a.CreatedOn.Date == day.Date);
                    result[label] = count;
                }
            }
            else if (period == "month")
            {
                var monthStart = new DateTime(now.Year, now.Month, 1);
                for (int i = 0; i < 4; i++)
                {
                    var weekStart = monthStart.AddDays(i * 7);
                    var weekEnd = weekStart.AddDays(6);
                    string label = $"Week {i + 1}";
                    int count = query.Count(a =>
                        a.CreatedOn >= weekStart && a.CreatedOn <= weekEnd);
                    result[label] = count;
                }
            }
            else if (period == "year")
            {
                for (int m = 1; m <= 12; m++)
                {
                    var start = new DateTime(now.Year, m, 1);
                    var end = start.AddMonths(1).AddSeconds(-1);
                    string label = start.ToString("MMMM", new System.Globalization.CultureInfo("en-US"));
                    int count = query.Count(a =>
                        a.CreatedOn >= start && a.CreatedOn <= end);
                    result[label] = count;
                }
            }

            return Json(result);
        }

    }
}
