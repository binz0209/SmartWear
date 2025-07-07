using Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository
{
    public interface ICartRepository
    {
        Task<IEnumerable<Cart>> GetAllCartsAsync();
        Task<Cart> GetCartByIdAsync(Guid id);
        Task AddCartAsync(Cart cart);
        Task UpdateCartAsync(Cart cart);
        Task DeleteCartAsync(Guid id);
    }
}
