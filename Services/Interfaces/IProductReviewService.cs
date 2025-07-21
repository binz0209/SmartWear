using Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IProductReviewService
    {
        Task<IEnumerable<ProductReview>> GetAllProductReviewsAsync();
        Task<ProductReview> GetProductReviewByIdAsync(Guid id);
        Task AddProductReviewAsync(ProductReview productReview);
        Task UpdateProductReviewAsync(ProductReview productReview);
        Task DeleteProductReviewAsync(Guid id);
    }
}
