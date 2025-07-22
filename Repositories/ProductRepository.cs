using Microsoft.EntityFrameworkCore;
using Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Data;
using Repositories.Interfaces;

namespace Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _context.Products.Include(p => p.Category).Include(p => p.OrderItems).Include(p => p.ProductReviews).ToListAsync();
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string keyword)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductReviews)
                .Where(p => !p.IsDeleted &&
                           (p.Name.Contains(keyword) ||
                            p.Description.Contains(keyword) ||
                            p.Price.ToString().Contains(keyword)))
                .ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(Guid id)
        {
            return await _context.Products.Include(p => p.Category)
        .Include(p => p.ProductReviews)
        .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddProductAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(Guid id)
        {
            var product = await GetProductByIdAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Product>> FilterProductsByColorsAsync(List<string> colors)
        {
            if (colors == null || !colors.Any())
                return new List<Product>();

            return await _context.Products
                .Where(p => colors.Contains(p.Color.ToLower()))
                .Include(p => p.Category)
                .ToListAsync();
        }

    }
}
