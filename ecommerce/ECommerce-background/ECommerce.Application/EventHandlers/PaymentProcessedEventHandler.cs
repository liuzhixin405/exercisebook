using ECommerce.Core.EventBus;
using ECommerce.Domain.Events;
using ECommerce.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.EventHandlers
{
    /// <summary>
    /// 支付处理事件处理器 - 简化版本，只做核心业务逻辑确认
    /// </summary>
    public class PaymentProcessedEventHandler : IEventHandler<PaymentProcessedEvent>
    {
        private readonly ILogger<PaymentProcessedEventHandler> _logger;
        private readonly INotificationService _notificationService;
        private readonly IStatisticsService _statisticsService;
        private readonly ICacheService _cacheService;

        public PaymentProcessedEventHandler(
            ILogger<PaymentProcessedEventHandler> logger,
            INotificationService notificationService,
            IStatisticsService statisticsService,
            ICacheService cacheService)
        {
            _logger = logger;
            _notificationService = notificationService;
            _statisticsService = statisticsService;
            _cacheService = cacheService;
        }

        public async Task<bool> HandleAsync(PaymentProcessedEvent domainEvent, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("PaymentProcessedEventHandler: Processing payment for order {OrderId}", domainEvent.OrderId);

                // 核心业务逻辑：支付处理确认
                await ProcessPaymentAsync(domainEvent, cancellationToken);

                _logger.LogInformation("PaymentProcessedEventHandler: Successfully processed payment for order {OrderId}", domainEvent.OrderId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PaymentProcessedEventHandler: Error processing payment for order {OrderId}", domainEvent.OrderId);
                return false;
            }
        }

        /// <summary>
        /// 处理支付的核心业务逻辑
        /// </summary>
        private async Task ProcessPaymentAsync(PaymentProcessedEvent domainEvent, CancellationToken cancellationToken)
        {
            // 1) 发送通知
            await _notificationService.SendPaymentNotificationAsync(domainEvent);

            // 2) 更新统计（仅成功时）
            if (domainEvent.Success)
            {
                await _statisticsService.UpdatePaymentStatisticsAsync(new OrderPaidEvent(
                    domainEvent.OrderId,
                    domainEvent.UserId,
                    domainEvent.PaymentId,
                    domainEvent.Amount,
                    domainEvent.PaymentMethod));
            }

            // 3) 支付失败时，可考虑释放锁定库存由上游处理（此处仅发通知/打点）
            if (!domainEvent.Success)
            {
                await _notificationService.SendSystemAlertAsync($"Payment failed: {domainEvent.PaymentId}", "Warning");
            }

            // 4) 失效订单相关缓存
            await _cacheService.RemoveByPatternAsync($"order:{domainEvent.OrderId}");
        }
    }
}
