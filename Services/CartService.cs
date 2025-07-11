using Repository;
using Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;

        public CartService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<IEnumerable<Cart>> GetAllCartsAsync()
        {
            return await _cartRepository.GetAllCartsAsync();
        }

        public async Task<Cart> GetCartByIdAsync(Guid id)
        {
            return await _cartRepository.GetCartByIdAsync(id);
        }

        public async Task<Cart> GetCartByUserIdAsync(Guid userId)
        {
            return await _cartRepository.GetCartByUserIdAsync(userId);
        }
        public async Task<Cart> GetCartWithItemsByUserIdAsync(Guid userId)
        {
            return await _cartRepository.GetCartWithItemsByUserIdAsync(userId);
        }

        public async Task AddCartAsync(Cart cart)
        {
            await _cartRepository.AddCartAsync(cart);
        }

        public async Task UpdateCartAsync(Cart cart)
        {
            await _cartRepository.UpdateCartAsync(cart);
        }

        public async Task DeleteCartAsync(Guid id)
        {
            await _cartRepository.DeleteCartAsync(id);
        }
    }
}
