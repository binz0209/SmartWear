using Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllProductsAsync();
        }

        public async Task<Product> GetProductByIdAsync(Guid id)
        {
            return await _productRepository.GetProductByIdAsync(id);
        }

        public async Task AddProductAsync(Product product)
        {
            await _productRepository.AddProductAsync(product);
        }

        public async Task UpdateProductAsync(Product product)
        {
            await _productRepository.UpdateProductAsync(product);
        }

        public async Task DeleteProductAsync(Guid id)
        {
            await _productRepository.DeleteProductAsync(id);
        }

        public async Task<IEnumerable<Product>> FilterProductsByColorsAsync(List<string> colors)
        {
            return await _productRepository.FilterProductsByColorsAsync(colors);
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string keyword)
        {
            return await _productRepository.SearchProductsAsync(keyword);
        }
        public async Task DecreaseProductQuantityAsync(Guid productId, int quantity)
        {
            await _productRepository.DecreaseProductQuantityAsync(productId, quantity);
        }
    }
}
