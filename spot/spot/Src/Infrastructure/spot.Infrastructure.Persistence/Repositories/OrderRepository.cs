using Microsoft.EntityFrameworkCore;
using spot.Application.Interfaces.Repositories.Orders;
using spot.Domain.Orders.Entities;
using spot.Domain.Orders.Enums;
using spot.Infrastructure.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace spot.Infrastructure.Persistence.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<Order> _orders;

        public OrderRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
            _orders = dbContext.Set<Order>();
        }

        public async Task<bool> CancelOrderAsync(string orderId)
        {
            var order = await GetByIdAsync(orderId);
            if (order == null || (order.Status != OrderStatus.Open && order.Status != OrderStatus.Pending))
            {
                return false;
            }

            order.Status = OrderStatus.Canceled;
            order.UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            _orders.Update(order);
            return true;
        }

        public async Task<IReadOnlyList<Order>> GetActiveOrdersByUserIdAsync(string userId)
        {
            return await _orders
                .Where(o => o.UserId == userId && 
                           (o.Status == OrderStatus.Open || o.Status == OrderStatus.Pending))
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Order>> GetOrdersByProductIdAsync(string productId)
        {
            return await _orders
                .Where(o => o.ProductId == productId)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Order>> GetOrdersByUserIdAsync(string userId)
        {
            return await _orders
                .Where(o => o.UserId == userId)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Order>> GetOrdersByUserIdAndProductIdAsync(string userId, string productId)
        {
            return await _orders
                .Where(o => o.UserId == userId && o.ProductId == productId)
                .ToListAsync();
        }

        public async Task<bool> UpdateFilledSizeAsync(string orderId, decimal additionalFilledSize, decimal additionalFilledFunds)
        {
            var order = await GetByIdAsync(orderId);
            if (order == null || order.Status != OrderStatus.Open)
            {
                return false;
            }

            order.FilledSize += additionalFilledSize;
            order.FilledFunds += additionalFilledFunds;
            order.UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            // Check if order is fully filled
            if (order.FilledSize >= order.Size)
            {
                order.Status = OrderStatus.Done;
            }

            _orders.Update(order);
            return true;
        }

        public async Task<bool> UpdateOrderStatusAsync(string orderId, string status)
        {
            var order = await GetByIdAsync(orderId);
            if (order == null)
            {
                return false;
            }

            order.Status = status;
            order.UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            _orders.Update(order);
            return true;
        }
    }
}