using ECommerce.Core.EventBus;
using ECommerce.Domain.Events;
using ECommerce.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using ECommerce.Infrastructure.Services;

namespace ECommerce.Application.EventHandlers
{
    /// <summary>
    /// 订单支付事件处理器 - 简化版本，只做核心业务逻辑确认
    /// </summary>
    public class OrderPaidEventHandler : IEventHandler<OrderPaidEvent>
    {
        private readonly ILogger<OrderPaidEventHandler> _logger;
        private readonly IStatisticsService _statisticsService;
        private readonly INotificationService _notificationService;
        private readonly ICacheService _cacheService;
        private readonly ECommerce.Domain.Interfaces.IOrderMessagePublisher _messagePublisher;

        public OrderPaidEventHandler(
            ILogger<OrderPaidEventHandler> logger,
            IStatisticsService statisticsService,
            INotificationService notificationService,
            ICacheService cacheService,
            ECommerce.Domain.Interfaces.IOrderMessagePublisher messagePublisher)
        {
            _logger = logger;
            _statisticsService = statisticsService;
            _notificationService = notificationService;
            _cacheService = cacheService;
            _messagePublisher = messagePublisher;
        }

        public async Task<bool> HandleAsync(OrderPaidEvent domainEvent, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("OrderPaidEventHandler: Processing order payment for order {OrderId}", domainEvent.OrderId);

                // 核心业务逻辑：订单支付确认
                await ProcessOrderPaymentAsync(domainEvent, cancellationToken);

                _logger.LogInformation("OrderPaidEventHandler: Successfully processed order payment for order {OrderId}", domainEvent.OrderId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OrderPaidEventHandler: Error processing order payment for order {OrderId}", domainEvent.OrderId);
                return false;
            }
        }

        /// <summary>
        /// 处理订单支付的核心业务逻辑
        /// </summary>
        private async Task ProcessOrderPaymentAsync(OrderPaidEvent domainEvent, CancellationToken cancellationToken)
        {
            // 1) 更新统计（支付/销售）
            await _statisticsService.UpdatePaymentStatisticsAsync(domainEvent);
            await _statisticsService.UpdateSalesStatisticsAsync(domainEvent);

            // 2) 发送支付成功通知
            await _notificationService.SendPaymentNotificationAsync(new PaymentProcessedEvent(
                domainEvent.PaymentId,
                domainEvent.OrderId,
                domainEvent.UserId,
                domainEvent.Amount,
                domainEvent.PaymentMethod,
                success: true));

            // 3) 失效相关订单缓存
            await _cacheService.RemoveByPatternAsync($"order:{domainEvent.OrderId}");
            await _cacheService.RemoveByPatternAsync($"orders:user:{domainEvent.UserId}");

            // 4) 发送订单确认消息（支付成功后自动确认订单）
            try
            {
                await _messagePublisher.PublishOrderConfirmationMessageAsync(domainEvent.OrderId, domainEvent.UserId);
                _logger.LogInformation("OrderPaidEventHandler: Sent order confirmation message for order {OrderId}", domainEvent.OrderId);
            }
            catch (Exception publishEx)
            {
                _logger.LogError(publishEx, "OrderPaidEventHandler: Failed to publish order confirmation message for order {OrderId}", domainEvent.OrderId);
            }
        }
    }
}

