using ECommerce.API.Application;
using ECommerce.API.Models;

namespace ECommerce.API.Infrastructure
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
            if (request == null || request.Items == null || !request.Items.Any())
            {
                _logger.LogWarning("Invalid order request received");
                throw new ArgumentException("Order request and items cannot be null or empty");
            }

            _logger.LogInformation("Creating order for customer {CustomerId} with {ItemCount} items", 
                request.CustomerId, request.Items.Count);
            
            try
            {
                OrderComposite orderComposite = new OrderComposite();
                request.Items.ForEach(orderItems =>
                {
                    orderComposite.AddComponent(new OrderItem() 
                    { 
                        ProductName = orderItems.ProductName, 
                        Price = orderItems.Price, 
                        Quantity = orderItems.Quantity 
                    });
                });

                var total = orderComposite.GetTotal();
                if (total <= 0)
                {
                    _logger.LogWarning("Invalid order total calculated: {Total}", total);
                    throw new InvalidOperationException("Order total must be greater than zero");
                }

                var order = new Order { CustomerId = request.CustomerId, Amount = total };
                await _repository.AddAsync(order);
                
                _logger.LogInformation("Order {OrderId} created successfully for customer {CustomerId}", 
                    order.Id, request.CustomerId);
                
                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order for customer {CustomerId}", request.CustomerId);
                throw;
            }
        }

        public async Task<Order> GetOrderAsync(int orderId)
        {
            return await _repository.GetByIdAsync(orderId);
        }
    }
}
