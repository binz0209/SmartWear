using Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public interface ICartService
    {
        Task<IEnumerable<Cart>> GetAllCartsAsync();
        Task<Cart> GetCartByIdAsync(Guid id);
        Task AddCartAsync(Cart cart);
        Task UpdateCartAsync(Cart cart);
        Task DeleteCartAsync(Guid id);
        Task<Cart> GetOrCreateCartAsync(Guid userId);
        Task AddToCartAsync(Guid userId, Guid productId, int quantity);
        Task<Cart> GetCartWithItemsByUserIdAsync(Guid userId);
    }
}
