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
    public class ProductReviewRepository : IProductReviewRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductReviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductReview>> GetAllProductReviewsAsync()
        {
            return await _context.ProductReviews.ToListAsync();
        }

        public async Task<ProductReview> GetProductReviewByIdAsync(Guid id)
        {
            return await _context.ProductReviews.FindAsync(id);
        }

        public async Task AddProductReviewAsync(ProductReview productReview)
        {
            await _context.ProductReviews.AddAsync(productReview);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductReviewAsync(ProductReview productReview)
        {
            _context.ProductReviews.Update(productReview);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductReviewAsync(Guid id)
        {
            var productReview = await GetProductReviewByIdAsync(id);
            if (productReview != null)
            {
                _context.ProductReviews.Remove(productReview);
                await _context.SaveChangesAsync();
            }
        }
    }
}
