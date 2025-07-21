using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Business.Data;
using Business.Models;
using Microsoft.AspNetCore.Http;

namespace SmartWearAdmin.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var products = _context.Products
                .Where(p => !p.IsDeleted)
                .Include(p => p.Category);

            return View(await products.ToListAsync());
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null) return NotFound();

            return View(product);
        }

        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile ImageFile)
        {
            if (string.IsNullOrWhiteSpace(product.Name) || product.Price <= 0 || product.StockQuantity < 0 || product.CategoryId == Guid.Empty)
            {
                ModelState.AddModelError("", "Please fill all required fields.");
                ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
                return View(product);
            }

            product.Id = Guid.NewGuid();

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "product");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                var extension = Path.GetExtension(ImageFile.FileName);
                var fileName = $"{product.Id}{extension}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                product.ImageUrl = $"/img/product/{fileName}";
            }

            product.CreatedOn = DateTime.UtcNow;

            _context.Add(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Product product, IFormFile ImageFile)
        {
            if (id != product.Id) return NotFound();

            if (string.IsNullOrWhiteSpace(product.Name) || product.Price <= 0 || product.StockQuantity < 0 || product.CategoryId == Guid.Empty)
            {
                ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
                ModelState.AddModelError("", "Please fill all required fields.");
                return View(product);
            }

            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null) return NotFound();

            // Update fields
            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.StockQuantity = product.StockQuantity;
            existingProduct.CategoryId = product.CategoryId;
            existingProduct.Size = product.Size;
            existingProduct.Color = product.Color;
            existingProduct.ModifiedOn = DateTime.UtcNow;

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "product");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                var extension = Path.GetExtension(ImageFile.FileName);
                var fileName = $"{existingProduct.Id}{extension}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                existingProduct.ImageUrl = $"/img/product/{fileName}";
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedAjax(Guid id)
        {
            var product = await _context.Products
                .Include(p => p.CartItems)
                .Include(p => p.OrderItems)
                .Include(p => p.ProductReviews)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product != null)
            {
                var now = DateTime.UtcNow;

                foreach (var item in product.CartItems) { item.IsDeleted = true; item.DeletedOn = now; }
                foreach (var item in product.OrderItems) { item.IsDeleted = true; item.DeletedOn = now; }
                foreach (var item in product.ProductReviews) { item.IsDeleted = true; item.DeletedOn = now; }

                product.IsDeleted = true;
                product.DeletedOn = now;

                await _context.SaveChangesAsync();
            }

            return Ok();
        }

        public async Task<IActionResult> Trash()
        {
            var deletedProducts = await _context.Products
                .Where(p => p.IsDeleted)
                .Include(p => p.Category)
                .ToListAsync();

            return View(deletedProducts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(Guid id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product != null)
            {
                var cartItems = await _context.CartItems.Where(c => c.ProductId == id).ToListAsync();
                foreach (var item in cartItems) { item.IsDeleted = false; item.DeletedOn = null; }

                var orderItems = await _context.OrderItems.Where(o => o.ProductId == id).ToListAsync();
                foreach (var item in orderItems) { item.IsDeleted = false; item.DeletedOn = null; }

                var reviews = await _context.ProductReviews.Where(r => r.ProductId == id).ToListAsync();
                foreach (var item in reviews) { item.IsDeleted = false; item.DeletedOn = null; }

                product.IsDeleted = false;
                product.DeletedOn = null;

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Trash));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CleanTrash()
        {
            var deletedProducts = await _context.Products
                .Where(p => p.IsDeleted)
                .Include(p => p.CartItems)
                .Include(p => p.OrderItems)
                .Include(p => p.ProductReviews)
                .ToListAsync();

            foreach (var product in deletedProducts)
            {
                _context.CartItems.RemoveRange(product.CartItems);
                _context.OrderItems.RemoveRange(product.OrderItems);
                _context.ProductReviews.RemoveRange(product.ProductReviews);
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Trash));
        }

        private bool ProductExists(Guid id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
