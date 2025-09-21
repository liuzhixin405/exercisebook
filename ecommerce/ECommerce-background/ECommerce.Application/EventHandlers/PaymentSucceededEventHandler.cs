using ECommerce.Core.EventBus;
using ECommerce.Domain.Events;
using ECommerce.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.EventHandlers
{
    public class PaymentSucceededEventHandler : IEventHandler<PaymentSucceededEvent>
    {
        private readonly ILogger<PaymentSucceededEventHandler> _logger;
        private readonly IStatisticsService _statisticsService;
        private readonly INotificationService _notificationService;
        private readonly ICacheService _cacheService;

        public PaymentSucceededEventHandler(
            ILogger<PaymentSucceededEventHandler> logger,
            IStatisticsService statisticsService,
            INotificationService notificationService,
            ICacheService cacheService)
        {
            _logger = logger;
            _statisticsService = statisticsService;
            _notificationService = notificationService;
            _cacheService = cacheService;
        }

        public async Task<bool> HandleAsync(PaymentSucceededEvent domainEvent, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("PaymentSucceededEventHandler: Processing payment success for order {OrderId}", domainEvent.OrderId);

                await _statisticsService.UpdatePaymentStatisticsAsync(new OrderPaidEvent(
                    domainEvent.OrderId,
                    domainEvent.UserId,
                    domainEvent.PaymentId,
                    domainEvent.Amount,
                    domainEvent.PaymentMethod));

                await _statisticsService.UpdateSalesStatisticsAsync(new OrderPaidEvent(
                    domainEvent.OrderId,
                    domainEvent.UserId,
                    domainEvent.PaymentId,
                    domainEvent.Amount,
                    domainEvent.PaymentMethod));

                await _notificationService.SendPaymentNotificationAsync(new PaymentProcessedEvent(
                    domainEvent.PaymentId,
                    domainEvent.OrderId,
                    domainEvent.UserId,
                    domainEvent.Amount,
                    domainEvent.PaymentMethod,
                    success: true));

                await _cacheService.RemoveByPatternAsync($"order:{domainEvent.OrderId}");
                await _cacheService.RemoveByPatternAsync($"orders:user:{domainEvent.UserId}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PaymentSucceededEventHandler: Error processing payment success for order {OrderId}", domainEvent.OrderId);
                return false;
            }
        }
    }
}


