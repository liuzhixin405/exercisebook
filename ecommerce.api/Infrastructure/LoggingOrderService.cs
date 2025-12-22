using ECommerce.API.Application;
using ECommerce.API.Models;
using System.Diagnostics;

namespace ECommerce.API.Infrastructure
{
    // 装饰器 - 日志增强
    public class LoggingOrderService : IOrderService
    {
        private readonly IOrderService _decoratedService;
        private readonly ILogger<LoggingOrderService> _logger;

        public LoggingOrderService(IOrderService decoratedService, ILogger<LoggingOrderService> logger)
        {
            _decoratedService = decoratedService;
            _logger = logger;
        }

        public async Task<Order> CreateOrderAsync(OrderRequest request)
        {
            _logger.LogInformation($"Creating order for customer {request.CustomerId}");
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var result = await _decoratedService.CreateOrderAsync(request);
                _logger.LogInformation($"Order created successfully in {stopwatch.ElapsedMilliseconds}ms");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create order");
                throw;
            }
        }

        public async Task<Order> GetOrderAsync(int orderId)
        {
            return await _decoratedService.GetOrderAsync(orderId);
        }
    }

}
