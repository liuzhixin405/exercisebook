using ECommerce.Core.EventBus;
using ECommerce.Domain.Events;
using ECommerce.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.EventHandlers
{
    /// <summary>
    /// 订单创建事件处理器 - 简化版本，只做核心业务逻辑确认
    /// </summary>
    public class OrderCreatedEventHandler : IEventHandler<OrderCreatedEvent>
    {
        private readonly ILogger<OrderCreatedEventHandler> _logger;
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly IStatisticsService _statisticsService;
        private readonly INotificationService _notificationService;
        private readonly ICacheService _cacheService;

        public OrderCreatedEventHandler(
            ILogger<OrderCreatedEventHandler> logger,
            IShoppingCartRepository shoppingCartRepository,
            IStatisticsService statisticsService,
            INotificationService notificationService,
            ICacheService cacheService)
        {
            _logger = logger;
            _shoppingCartRepository = shoppingCartRepository;
            _statisticsService = statisticsService;
            _notificationService = notificationService;
            _cacheService = cacheService;
        }

        public async Task<bool> HandleAsync(OrderCreatedEvent domainEvent, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("OrderCreatedEventHandler: Processing order creation for order {OrderId}", domainEvent.OrderId);

                // 核心业务逻辑：订单创建确认
                await ProcessOrderCreationAsync(domainEvent, cancellationToken);

                _logger.LogInformation("OrderCreatedEventHandler: Successfully processed order creation for order {OrderId}", domainEvent.OrderId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OrderCreatedEventHandler: Error processing order creation for order {OrderId}", domainEvent.OrderId);
                return false;
            }
        }

        /// <summary>
        /// 处理订单创建的核心业务逻辑
        /// </summary>
        private async Task ProcessOrderCreationAsync(OrderCreatedEvent domainEvent, CancellationToken cancellationToken)
        {
            // 1) 清理用户购物车
            var cart = await _shoppingCartRepository.GetByUserIdAsync(domainEvent.UserId);
            if (cart != null)
            {
                await _shoppingCartRepository.ClearCartAsync(cart.Id);
                _logger.LogInformation("Cleared shopping cart {CartId} for user {UserId} after order {OrderId} creation", cart.Id, domainEvent.UserId, domainEvent.OrderId);
            }

            // 2) 更新统计
            await _statisticsService.UpdateOrderStatisticsAsync(domainEvent);

            // 3) 通知（异步）
            await _notificationService.SendOrderStatusNotificationAsync(domainEvent.OrderId.ToString(), domainEvent.UserId.ToString(), "Created");

            // 4) 失效与订单相关的缓存
            await _cacheService.RemoveByPatternAsync($"orders:user:{domainEvent.UserId}");
            await _cacheService.RemoveByPatternAsync($"order:{domainEvent.OrderId}");
        }
    }
}