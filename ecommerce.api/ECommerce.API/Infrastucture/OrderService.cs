using ECommerce.API.Application;
using ECommerce.API.Models;

namespace ECommerce.API.Infrastucture
{
    // 基础实现
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repository;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IOrderRepository repository, ILogger<OrderService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<Order> CreateOrderAsync(OrderRequest request)
        {
            _logger.LogInformation("Creating order...");
            var order = new Order { CustomerId = request.CustomerId, Amount = request.Amount };
            await _repository.AddAsync(order);
            return order;
        }

        public async Task<Order> GetOrderAsync(int orderId)
        {
            return await _repository.GetByIdAsync(orderId);
        }
    }
}
