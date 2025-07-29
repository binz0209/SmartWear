using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Business.Data;
using Business.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SmartWearAdmin.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(ApplicationDbContext context, ILogger<OrdersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Address)
                .Where(o => !o.IsDeleted)
                .ToListAsync();
            return View(orders);
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();

            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Address)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);

            if (order == null) return NotFound();

            return View(order);
        }

        // POST: Orders/Delete (soft-delete)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                order.IsDeleted = true;
                order.DeletedOn = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Orders/Trash
        public async Task<IActionResult> Trash()
        {
            var orders = await _context.Orders
                .Where(o => o.IsDeleted)
                .Include(o => o.User)
                .Include(o => o.Address)
                .ToListAsync();
            return View(orders);
        }

        // POST: Orders/Restore
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                order.IsDeleted = false;
                order.DeletedOn = null;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Orders/DeletePermanently
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePermanently(Guid id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order != null)
            {
                if (order.OrderItems != null) _context.OrderItems.RemoveRange(order.OrderItems);
                if (order.Payment != null) _context.Payments.Remove(order.Payment);
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Trash));
        }

        // POST: Orders/CleanTrash
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CleanTrash()
        {
            var deletedOrders = await _context.Orders
                .Where(o => o.IsDeleted)
                .Include(o => o.OrderItems)
                .Include(o => o.Payment)
                .ToListAsync();

            foreach (var order in deletedOrders)
            {
                if (order.OrderItems != null) _context.OrderItems.RemoveRange(order.OrderItems);
                if (order.Payment != null) _context.Payments.Remove(order.Payment);
            }
            _context.Orders.RemoveRange(deletedOrders);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Trash));
        }
        [HttpGet]
        public async Task<IActionResult> EditStatus(Guid? id)
        {
            if (id == null) return NotFound();
            var order = await _context.Orders.Include(o => o.User).FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted);
            if (order == null) return NotFound();
            ViewBag.StatusList = new SelectList(new[] { "Confirmed", "In delivery", "Delivered" }, order.Status);
            return View(order);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStatus(Guid id, [Bind("Id,Status")] Order order)
        {
            ModelState.Remove("OrderDate");
            ModelState.Remove("UserId");
            ModelState.Remove("AddressId");
            ModelState.Remove("User");
            ModelState.Remove("Address");
            ModelState.Remove("Payment");
            ModelState.Remove("OrderItems");

            // DEBUG ModelState lỗi
            foreach (var key in ModelState.Keys)
            {
                var state = ModelState[key];
                if (state.Errors.Count > 0)
                {
                    _logger.LogWarning("ModelState error: {Key} - {Error}",
                        key,
                        string.Join(" | ", state.Errors.Select(e => e.ErrorMessage)));
                }
            }

            if (!ModelState.IsValid)
            {
                var orderDb = await _context.Orders.Include(o => o.User).FirstOrDefaultAsync(o => o.Id == id);
                if (orderDb == null) return NotFound();
                ViewBag.StatusList = new SelectList(new[] { "Confirmed", "In delivery", "Delivered" }, order.Status);
                orderDb.Status = order.Status;
                return View(orderDb);
            }

            var orderToUpdate = await _context.Orders.FindAsync(id);
            if (orderToUpdate == null) return NotFound();
            orderToUpdate.Status = order.Status;
            orderToUpdate.ModifiedOn = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
