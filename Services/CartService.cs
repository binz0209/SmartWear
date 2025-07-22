using Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemRepository _cartItemRepository;


        public CartService(ICartRepository cartRepository, ICartItemRepository cartItemRepository)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
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
        public async Task ClearCartAsync(Guid cartId)
        {
            await _cartRepository.ClearCartAsync(cartId);
        }
        public async Task<Cart> GetOrCreateCartAsync(Guid userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                cart = new Cart
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false
                };
                await _cartRepository.AddCartAsync(cart);
            }
            return cart;
        }
        public async Task AddToCartAsync(Guid userId, Guid productId, int quantity)
        {
            var cart = await GetOrCreateCartAsync(userId);

            var existingItem = await _cartItemRepository.GetCartItemByCartIdAndProductIdAsync(cart.Id, productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                await _cartItemRepository.UpdateCartItemAsync(existingItem);
            }
            else
            {
                var newItem = new CartItem
                {
                    Id = Guid.NewGuid(),
                    CartId = cart.Id,
                    ProductId = productId,
                    Quantity = quantity,
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false
                };
                await _cartItemRepository.AddCartItemAsync(newItem);
            }
        }
    }
}
