using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Business.Data;
using Business.Models;

namespace SmartWearAdmin.Controllers
{
    public class ChatLogsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChatLogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // INDEX: Liệt kê từng user có chatlog (group by User)
        public async Task<IActionResult> Index()
        {
            var userChats = await _context.ChatLogs
                .Where(cl => !cl.IsDeleted)
                .GroupBy(cl => cl.User)
                .Select(g => new
                {
                    User = g.Key,
                    ChatCount = g.Count(),
                    LastChat = g.Max(x => x.CreatedOn)
                })
                .OrderByDescending(x => x.LastChat)
                .ToListAsync();

            return View(userChats);
        }

        // Xem chi tiết chat của từng user
        public async Task<IActionResult> UserChat(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            var chatLogs = await _context.ChatLogs
                .Where(cl => cl.UserId == userId && !cl.IsDeleted)
                .OrderBy(cl => cl.CreatedOn)
                .ToListAsync();

            ViewBag.User = user;
            return View(chatLogs);
        }

        // Xóa mềm chatlog của user (set IsDeleted=true cho tất cả chatlog của user)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDelete(Guid userId)
        {
            var chatLogs = await _context.ChatLogs.Where(cl => cl.UserId == userId && !cl.IsDeleted).ToListAsync();
            foreach (var cl in chatLogs)
            {
                cl.IsDeleted = true;
                cl.DeletedOn = DateTime.UtcNow;
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Trash: Liệt kê user có chatlog đã bị xóa mềm
        public async Task<IActionResult> Trash()
        {
            var userChats = await _context.ChatLogs
                .Where(cl => cl.IsDeleted)
                .GroupBy(cl => cl.User)
                .Select(g => new
                {
                    User = g.Key,
                    ChatCount = g.Count(),
                    LastDeleted = g.Max(x => x.DeletedOn)
                })
                .OrderByDescending(x => x.LastDeleted)
                .ToListAsync();

            return View(userChats);
        }

        // Khôi phục toàn bộ chatlog của user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(Guid userId)
        {
            var chatLogs = await _context.ChatLogs.Where(cl => cl.UserId == userId && cl.IsDeleted).ToListAsync();
            foreach (var cl in chatLogs)
            {
                cl.IsDeleted = false;
                cl.DeletedOn = null;
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Trash));
        }

        // Xóa vĩnh viễn toàn bộ chatlog của user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePermanently(Guid userId)
        {
            var chatLogs = await _context.ChatLogs.Where(cl => cl.UserId == userId && cl.IsDeleted).ToListAsync();
            _context.ChatLogs.RemoveRange(chatLogs);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Trash));
        }
    }
}
