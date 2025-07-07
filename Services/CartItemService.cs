using Repository;
using Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public class CartItemService : ICartItemService
    {
        private readonly ICartItemRepository _cartItemRepository;

        public CartItemService(ICartItemRepository cartItemRepository)
        {
            _cartItemRepository = cartItemRepository;
        }

        public async Task<IEnumerable<CartItem>> GetAllCartItemsAsync()
        {
            return await _cartItemRepository.GetAllCartItemsAsync();
        }

        public async Task<CartItem> GetCartItemByIdAsync(Guid id)
        {
            return await _cartItemRepository.GetCartItemByIdAsync(id);
        }

        public async Task AddCartItemAsync(CartItem cartItem)
        {
            await _cartItemRepository.AddCartItemAsync(cartItem);
        }

        public async Task UpdateCartItemAsync(CartItem cartItem)
        {
            await _cartItemRepository.UpdateCartItemAsync(cartItem);
        }

        public async Task DeleteCartItemAsync(Guid id)
        {
            await _cartItemRepository.DeleteCartItemAsync(id);
        }
    }
}
