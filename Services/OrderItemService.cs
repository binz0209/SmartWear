using Repository;
using Business.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public class OrderItemService : IOrderItemService
    {
        private readonly IOrderItemRepository _orderItemRepository;

        public OrderItemService(IOrderItemRepository orderItemRepository)
        {
            _orderItemRepository = orderItemRepository;
        }

        public async Task<IEnumerable<OrderItem>> GetAllOrderItemsAsync()
        {
            return await _orderItemRepository.GetAllOrderItemsAsync();
        }

        public async Task<OrderItem> GetOrderItemByIdAsync(Guid id)
        {
            return await _orderItemRepository.GetOrderItemByIdAsync(id);
        }

        public async Task AddOrderItemAsync(OrderItem orderItem)
        {
            await _orderItemRepository.AddOrderItemAsync(orderItem);
        }

        public async Task UpdateOrderItemAsync(OrderItem orderItem)
        {
            await _orderItemRepository.UpdateOrderItemAsync(orderItem);
        }

        public async Task DeleteOrderItemAsync(Guid id)
        {
            await _orderItemRepository.DeleteOrderItemAsync(id);
        }
    }
}
