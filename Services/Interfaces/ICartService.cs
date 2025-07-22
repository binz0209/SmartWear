using Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ICartService
    {
        Task<IEnumerable<Cart>> GetAllCartsAsync();
        Task<Cart> GetCartByIdAsync(Guid id);
        Task<Cart> GetCartByUserIdAsync(Guid userId);
        Task<Cart> GetCartWithItemsByUserIdAsync(Guid userId);
        Task AddCartAsync(Cart cart);
        Task UpdateCartAsync(Cart cart);
        Task DeleteCartAsync(Guid id);
        Task ClearCartAsync(Guid cartId);
        Task<Cart> GetOrCreateCartAsync(Guid userId);
        Task AddToCartAsync(Guid userId, Guid productId, int quantity);
    }
}
