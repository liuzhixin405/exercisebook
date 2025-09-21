using ECommerce.Core.EventBus;
using ECommerce.Domain.Events;
using ECommerce.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.EventHandlers
{
    /// <summary>
    /// 订单取消事件处理器 - 简化版本，只做核心业务逻辑确认
    /// </summary>
    public class OrderCancelledEventHandler : IEventHandler<OrderCancelledEvent>
    {
        private readonly ILogger<OrderCancelledEventHandler> _logger;
        private readonly IStatisticsService _statisticsService;
        private readonly INotificationService _notificationService;
        private readonly ICacheService _cacheService;

        public OrderCancelledEventHandler(
            ILogger<OrderCancelledEventHandler> logger,
            IStatisticsService statisticsService,
            INotificationService notificationService,
            ICacheService cacheService)
        {
            _logger = logger;
            _statisticsService = statisticsService;
            _notificationService = notificationService;
            _cacheService = cacheService;
        }

        public async Task<bool> HandleAsync(OrderCancelledEvent domainEvent, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("OrderCancelledEventHandler: Processing order cancellation for order {OrderId}", domainEvent.OrderId);

                // 核心业务逻辑：订单取消确认
                await ProcessOrderCancellationAsync(domainEvent, cancellationToken);

                _logger.LogInformation("OrderCancelledEventHandler: Successfully processed order cancellation for order {OrderId}", domainEvent.OrderId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OrderCancelledEventHandler: Error processing order cancellation for order {OrderId}", domainEvent.OrderId);
                return false;
            }
        }

        /// <summary>
        /// 处理订单取消的核心业务逻辑
        /// </summary>
        private async Task ProcessOrderCancellationAsync(OrderCancelledEvent domainEvent, CancellationToken cancellationToken)
        {
            // 1. 记录订单取消日志
            _logger.LogInformation("Order {OrderId} cancelled with reason: {Reason} at {Timestamp}", 
                domainEvent.OrderId, domainEvent.Reason, domainEvent.OccurredOn);

            // 2) 更新统计
            await _statisticsService.UpdateCancellationStatisticsAsync(domainEvent);

            // 3) 发送通知
            await _notificationService.SendOrderStatusNotificationAsync(domainEvent.OrderId.ToString(), domainEvent.UserId.ToString(), "Cancelled");

            // 4) 失效缓存
            await _cacheService.RemoveByPatternAsync($"order:{domainEvent.OrderId}");
            await _cacheService.RemoveByPatternAsync($"orders:user:{domainEvent.UserId}");

            _logger.LogInformation("Order cancellation handled for order {OrderId}", domainEvent.OrderId);
        }
    }
}
