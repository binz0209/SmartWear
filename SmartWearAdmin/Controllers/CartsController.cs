using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Business.Data;
using Business.Models;
using Microsoft.Extensions.Logging;

namespace SmartWearAdmin.Controllers
{
    public class CartsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CartsController> _logger;

        public CartsController(ApplicationDbContext context, ILogger<CartsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Carts
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Truy cập danh sách Cart (Index).");
            var carts = await _context.Carts.Include(c => c.User).Where(c => !c.IsDeleted).ToListAsync();
            return View(carts);
        }

        // GET: Carts/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Truy cập Details với id null.");
                return NotFound();
            }

            var cart = await _context.Carts
                .Include(c => c.User)
                .Include(c => c.CartItems).ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);

            if (cart == null)
            {
                _logger.LogWarning("Không tìm thấy Cart với id: {CartId}", id);
                return NotFound();
            }

            return View(cart);
        }
        public IActionResult Create()
        {
            _logger.LogInformation("Accessed Cart creation page.");

            // Get users who do not have any Cart (UserId not in Carts table or all their carts are soft-deleted)
            var usersWithoutCart = _context.Users
                .Where(u => !_context.Carts.Any(c => c.UserId == u.Id && !c.IsDeleted))
                .ToList();

            ViewData["UserId"] = new SelectList(usersWithoutCart, "Id", "Email");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cart cart)
        {
            ModelState.Remove("User");
            ModelState.Remove("CartItems");

            if (cart.UserId == Guid.Empty)
            {
                ModelState.AddModelError("UserId", "Please select a user.");
            }

            if (ModelState.IsValid)
            {
                cart.Id = Guid.NewGuid();
                cart.CreatedOn = DateTime.UtcNow;
                cart.ModifiedOn = DateTime.UtcNow;
                cart.IsDeleted = false;
                cart.DeletedOn = null;

                try
                {
                    _context.Carts.Add(cart);
                    var affected = await _context.SaveChangesAsync();
                    _logger.LogInformation("Đã lưu {Count} cart vào database.", affected);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi lưu cart vào database.");
                    ModelState.AddModelError("", "Đã có lỗi khi lưu giỏ hàng.");
                }
            }

            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", cart.UserId);
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePermanently(Guid id)
        {
            // Load cart kèm CartItems nếu có
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cart != null)
            {
                // Xoá các CartItem liên quan (nếu có)
                if (cart.CartItems != null && cart.CartItems.Any())
                {
                    _context.CartItems.RemoveRange(cart.CartItems);
                }
                // Xoá cart
                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Trash));
        }


        // POST: Carts/Delete/5 (soft delete)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart != null)
            {
                cart.IsDeleted = true;
                cart.DeletedOn = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Cart {CartId} đã soft delete.", id);
            }
            else
            {
                _logger.LogWarning("Không tìm thấy Cart khi soft delete với id: {CartId}", id);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Carts/DeleteConfirmedAjax
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmedAjax(Guid id)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart != null)
            {
                cart.IsDeleted = true;
                cart.DeletedOn = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Cart {CartId} đã soft delete qua Ajax.", id);
                return Ok();
            }
            _logger.LogWarning("Không tìm thấy Cart khi soft delete Ajax với id: {CartId}", id);
            return NotFound();
        }

        // POST: Carts/Restore
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(Guid id)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart != null)
            {
                cart.IsDeleted = false;
                cart.DeletedOn = null;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Cart {CartId} đã được restore.", id);
            }
            else
            {
                _logger.LogWarning("Không tìm thấy Cart để restore với id: {CartId}", id);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Carts/Trash
        public async Task<IActionResult> Trash()
        {
            _logger.LogInformation("Truy cập Trash (Cart đã xóa mềm).");
            var carts = await _context.Carts
                .Where(c => c.IsDeleted)
                .Include(c => c.User)
                .ToListAsync();
            return View(carts);
        }

        // POST: Carts/CleanTrash
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CleanTrash()
        {
            var deletedCarts = await _context.Carts
                .Where(c => c.IsDeleted)
                .ToListAsync();

            _context.Carts.RemoveRange(deletedCarts);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Đã xóa vĩnh viễn {Count} Cart đã soft delete.", deletedCarts.Count);

            return RedirectToAction(nameof(Trash));
        }

        private bool CartExists(Guid id)
        {
            return _context.Carts.Any(e => e.Id == id);
        }
    }
}
