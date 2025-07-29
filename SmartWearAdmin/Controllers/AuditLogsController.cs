using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Business.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmartWearAdmin.Controllers
{
    public class AuditLogsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuditLogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AuditLogs
        public async Task<IActionResult> Index(string actionFilter)
        {
            // Lấy danh sách action distinct
            var actions = await _context.AuditLogs
                .Select(a => a.Action)
                .Distinct()
                .OrderBy(a => a)
                .ToListAsync();

            // Query logs + filter nếu có
            var logsQuery = _context.AuditLogs
                .Include(a => a.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(actionFilter))
            {
                logsQuery = logsQuery.Where(l => l.Action == actionFilter);
            }

            // Sắp xếp: cũ nhất lên trên, mới nhất xuống dưới
            var logs = await logsQuery
                .OrderBy(l => l.CreatedOn)
                .ToListAsync();

            // Truyền filter + list action sang view
            ViewBag.Actions = actions;
            ViewBag.SelectedAction = actionFilter;
            return View(logs);
        }
    }
}
