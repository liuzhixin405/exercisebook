using spot.Domain.Orders.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace spot.Application.Interfaces.Repositories.Orders
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<IReadOnlyList<Order>> GetOrdersByUserIdAsync(string userId);
        Task<IReadOnlyList<Order>> GetOrdersByProductIdAsync(string productId);
        Task<IReadOnlyList<Order>> GetActiveOrdersByUserIdAsync(string userId);
        Task<IReadOnlyList<Order>> GetOrdersByUserIdAndProductIdAsync(string userId, string productId);
        Task<bool> CancelOrderAsync(string orderId);
        Task<bool> UpdateOrderStatusAsync(string orderId, string status);
        Task<bool> UpdateFilledSizeAsync(string orderId, decimal additionalFilledSize, decimal additionalFilledFunds);
    }
}