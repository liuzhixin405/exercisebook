using ECommerce.Core.EventBus;
using ECommerce.Domain.Events;
using ECommerce.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.EventHandlers
{
    public class PaymentFailedEventHandler : IEventHandler<PaymentFailedEvent>
    {
        private readonly ILogger<PaymentFailedEventHandler> _logger;
        private readonly INotificationService _notificationService;
        private readonly ICacheService _cacheService;

        public PaymentFailedEventHandler(
            ILogger<PaymentFailedEventHandler> logger,
            INotificationService notificationService,
            ICacheService cacheService)
        {
            _logger = logger;
            _notificationService = notificationService;
            _cacheService = cacheService;
        }

        public async Task<bool> HandleAsync(PaymentFailedEvent domainEvent, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogWarning("PaymentFailedEventHandler: Payment failed for order {OrderId}, error: {Error}", domainEvent.OrderId, domainEvent.ErrorMessage);

                await _notificationService.SendPaymentNotificationAsync(new PaymentProcessedEvent(
                    domainEvent.PaymentId,
                    domainEvent.OrderId,
                    domainEvent.UserId,
                    domainEvent.Amount,
                    domainEvent.PaymentMethod,
                    success: false,
                    errorMessage: domainEvent.ErrorMessage));

                await _notificationService.SendSystemAlertAsync($"Payment failed for order {domainEvent.OrderId}: {domainEvent.ErrorMessage}", "Warning");

                await _cacheService.RemoveByPatternAsync($"order:{domainEvent.OrderId}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PaymentFailedEventHandler: Error handling payment failure for order {OrderId}", domainEvent.OrderId);
                return false;
            }
        }
    }
}


