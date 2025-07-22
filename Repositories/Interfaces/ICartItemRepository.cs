using Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface ICartItemRepository
    {
        Task<IEnumerable<CartItem>> GetAllCartItemsAsync();
        Task<CartItem> GetCartItemByIdAsync(Guid id);
        Task AddCartItemAsync(CartItem cartItem);
        Task UpdateCartItemAsync(CartItem cartItem);
        Task DeleteCartItemAsync(Guid cartItemId);
        Task<CartItem?> GetCartItemByCartIdAndProductIdAsync(Guid cartId, Guid productId);
        Task<IEnumerable<CartItem>> GetCartItemsByCartIdAsync(Guid cartId);
    }
}
