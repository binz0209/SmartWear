using Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(Guid id);
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(Guid id);
        Task<IEnumerable<Product>> FilterProductsByColorsAsync(List<string> colors);
        Task<IEnumerable<Product>> SearchProductsAsync(string keyword);
        Task DecreaseProductQuantityAsync(Guid productId, int quantity);
    }
}
