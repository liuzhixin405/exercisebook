using ECommerce.API.Application;
using ECommerce.API.Models;
using Microsoft.Extensions.Caching.Memory;

namespace ECommerce.API.Infrastucture
{
    // 装饰器 - 缓存增强
    public class CachedOrderService : IOrderService
    {
        private readonly IOrderService _decoratedService;
        private readonly IMemoryCache _cache;
        private readonly ILogger<CachedOrderService> _logger;

        public CachedOrderService(
            IOrderService decoratedService,
            IMemoryCache cache,
            ILogger<CachedOrderService> logger)
        {
            _decoratedService = decoratedService;
            _cache = cache;
            _logger = logger;
        }

        public async Task<Order> CreateOrderAsync(OrderRequest request)
        {
            var order = await _decoratedService.CreateOrderAsync(request);
            _cache.Remove($"order_{order.Id}");
            return order;
        }

        public async Task<Order> GetOrderAsync(int orderId)
        {
            string cacheKey = $"order_{orderId}";

            if (_cache.TryGetValue(cacheKey, out Order cachedOrder))
            {
                _logger.LogInformation("Returning cached order");
                return cachedOrder;
            }

            var order = await _decoratedService.GetOrderAsync(orderId);
            _cache.Set(cacheKey, order, TimeSpan.FromMinutes(5));

            return order;
        }
    }

}
