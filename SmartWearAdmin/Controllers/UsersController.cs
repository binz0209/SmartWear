using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Business.Data;
using Business.Models;
using BCrypt.Net;

namespace SmartWearAdmin.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _context.Users
                .Where(u => !u.IsDeleted)
                .Include(u => u.Role)
                .ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> Trash()
        {
            var users = await _context.Users
                .Where(u => u.IsDeleted)
                .Include(u => u.Role)
                .ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (user == null) return NotFound();

            return View(user);
        }

        public IActionResult Create()
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user, string PlainPassword)
        {
            if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(PlainPassword) || user.RoleId == Guid.Empty)
            {
                ModelState.AddModelError("", "Please fill all required fields.");
                ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", user.RoleId);
                return View(user);
            }

            user.Id = Guid.NewGuid();
            user.CreatedOn = DateTime.UtcNow;
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(PlainPassword);

            _context.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", user.RoleId);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, User user, string NewPassword)
        {
            if (id != user.Id) return NotFound();

            if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Email) || user.RoleId == Guid.Empty)
            {
                ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", user.RoleId);
                ModelState.AddModelError("", "Please fill all required fields.");
                return View(user);
            }

            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null) return NotFound();

            existingUser.Username = user.Username;
            existingUser.Email = user.Email;
            existingUser.RoleId = user.RoleId;
            existingUser.ModifiedOn = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(NewPassword))
            {
                existingUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(NewPassword);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedAjax(Guid id)
        {
            var user = await _context.Users
                .Include(u => u.Addresses)
                .Include(u => u.Orders)
                .Include(u => u.ChatLogs)
                .Include(u => u.ProductReviews)
                .Include(u => u.Cart)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user != null)
            {
                var now = DateTime.UtcNow;
                foreach (var addr in user.Addresses) { addr.IsDeleted = true; addr.DeletedOn = now; }
                foreach (var order in user.Orders) { order.IsDeleted = true; order.DeletedOn = now; }
                foreach (var log in user.ChatLogs) { log.IsDeleted = true; log.DeletedOn = now; }
                foreach (var review in user.ProductReviews) { review.IsDeleted = true; review.DeletedOn = now; }
                if (user.Cart != null) { user.Cart.IsDeleted = true; user.Cart.DeletedOn = now; }

                user.IsDeleted = true;
                user.DeletedOn = now;

                await _context.SaveChangesAsync();
            }

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(Guid id)
        {
            var user = await _context.Users
                .Include(u => u.Addresses)
                .Include(u => u.Orders)
                .Include(u => u.ChatLogs)
                .Include(u => u.ProductReviews)
                .Include(u => u.Cart)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user != null)
            {
                foreach (var addr in user.Addresses) { addr.IsDeleted = false; addr.DeletedOn = null; }
                foreach (var order in user.Orders) { order.IsDeleted = false; order.DeletedOn = null; }
                foreach (var log in user.ChatLogs) { log.IsDeleted = false; log.DeletedOn = null; }
                foreach (var review in user.ProductReviews) { review.IsDeleted = false; review.DeletedOn = null; }
                if (user.Cart != null) { user.Cart.IsDeleted = false; user.Cart.DeletedOn = null; }

                user.IsDeleted = false;
                user.DeletedOn = null;

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Trash));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CleanTrash()
        {
            var deletedUsers = await _context.Users
                .Where(u => u.IsDeleted)
                .Include(u => u.Addresses)
                .Include(u => u.Orders)
                .Include(u => u.ChatLogs)
                .Include(u => u.ProductReviews)
                .Include(u => u.Cart)
                .ToListAsync();

            foreach (var user in deletedUsers)
            {
                _context.Addresses.RemoveRange(user.Addresses);
                _context.Orders.RemoveRange(user.Orders);
                _context.ChatLogs.RemoveRange(user.ChatLogs);
                _context.ProductReviews.RemoveRange(user.ProductReviews);
                if (user.Cart != null) _context.Carts.Remove(user.Cart);
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Trash));
        }

        private bool UserExists(Guid id) => _context.Users.Any(e => e.Id == id);
    }
}