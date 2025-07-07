using Repository;
using Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public class ProductReviewService : IProductReviewService
    {
        private readonly IProductReviewRepository _productReviewRepository;

        public ProductReviewService(IProductReviewRepository productReviewRepository)
        {
            _productReviewRepository = productReviewRepository;
        }

        public async Task<IEnumerable<ProductReview>> GetAllProductReviewsAsync()
        {
            return await _productReviewRepository.GetAllProductReviewsAsync();
        }

        public async Task<ProductReview> GetProductReviewByIdAsync(Guid id)
        {
            return await _productReviewRepository.GetProductReviewByIdAsync(id);
        }

        public async Task AddProductReviewAsync(ProductReview productReview)
        {
            await _productReviewRepository.AddProductReviewAsync(productReview);
        }

        public async Task UpdateProductReviewAsync(ProductReview productReview)
        {
            await _productReviewRepository.UpdateProductReviewAsync(productReview);
        }

        public async Task DeleteProductReviewAsync(Guid id)
        {
            await _productReviewRepository.DeleteProductReviewAsync(id);
        }
    }
}
