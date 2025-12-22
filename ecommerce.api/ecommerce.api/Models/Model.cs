using ECommerce.API.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Models
{
    public class OrderRequest
    {
        public string CustomerId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentType { get; set; }
        public List<OrderItem> Items { get; set; }
    }

    public class Order
    {
        public int Id { get; set; }
        public string CustomerId { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PaymentResult
    {
        public bool Success { get; set; }
        public Guid TransactionId { get; set; }
        public string ErrorMessage { get; set; }
    }

    public interface IOrderRepository
    {
        Task<Order> GetByIdAsync(int id);
        Task AddAsync(Order order);
        Task<List<Order>> GetOrdersByCustomerIdAsync(string customerId);
        Task<List<Order>> GetAllOrdersAsync();
        Task UpdateAsync(Order order);
        Task DeleteAsync(int id);
    }

    public class OrderRepository : IOrderRepository
    {
        private readonly ECommerceDbContext _context;
        private readonly ILogger<OrderRepository> _logger;

        public OrderRepository(ECommerceDbContext context, ILogger<OrderRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Order> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Fetching order with id: {OrderId}", id);
                var order = await _context.Orders.FindAsync(id);
                if (order == null)
                {
                    _logger.LogWarning("Order with id {OrderId} not found", id);
                }
                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching order with id: {OrderId}", id);
                throw;
            }
        }

        public async Task AddAsync(Order order)
        {
            try
            {
                _logger.LogInformation("Adding new order for customer {CustomerId}", order.CustomerId);
                order.CreatedAt = DateTime.UtcNow;
                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Order added successfully with id: {OrderId}", order.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding order for customer {CustomerId}", order.CustomerId);
                throw;
            }
        }

        public async Task<List<Order>> GetOrdersByCustomerIdAsync(string customerId)
        {
            try
            {
                _logger.LogInformation("Fetching orders for customer {CustomerId}", customerId);
                var orders = await _context.Orders
                    .Where(o => o.CustomerId == customerId)
                    .OrderByDescending(o => o.CreatedAt)
                    .ToListAsync();
                return orders;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching orders for customer {CustomerId}", customerId);
                throw;
            }
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all orders");
                var orders = await _context.Orders
                    .OrderByDescending(o => o.CreatedAt)
                    .ToListAsync();
                return orders;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all orders");
                throw;
            }
        }

        public async Task UpdateAsync(Order order)
        {
            try
            {
                _logger.LogInformation("Updating order with id: {OrderId}", order.Id);
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Order updated successfully with id: {OrderId}", order.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order with id: {OrderId}", order.Id);
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deleting order with id: {OrderId}", id);
                var order = await _context.Orders.FindAsync(id);
                if (order != null)
                {
                    _context.Orders.Remove(order);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Order deleted successfully with id: {OrderId}", id);
                }
                else
                {
                    _logger.LogWarning("Order with id {OrderId} not found for deletion", id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting order with id: {OrderId}", id);
                throw;
            }
        }
    }
}
