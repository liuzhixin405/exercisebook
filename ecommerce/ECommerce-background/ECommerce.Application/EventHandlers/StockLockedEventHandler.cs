using ECommerce.Core.EventBus;
using ECommerce.Domain.Events;
using ECommerce.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.EventHandlers
{
    public class StockLockedEventHandler : IEventHandler<StockLockedEvent>
    {
        private readonly ILogger<StockLockedEventHandler> _logger;
        private readonly INotificationService _notificationService;

        public StockLockedEventHandler(
            ILogger<StockLockedEventHandler> logger,
            INotificationService notificationService)
        {
            _logger = logger;
            _notificationService = notificationService;
        }

        public async Task<bool> HandleAsync(StockLockedEvent domainEvent, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("StockLockedEventHandler: Stock locked for product {ProductId}, order {OrderId}, qty {Qty}", domainEvent.ProductId, domainEvent.OrderId, domainEvent.Quantity);

                // 可接入监控/审计/补偿定时器（此处先发监控通知）
                await _notificationService.SendInventoryLockNotificationAsync(new InventoryUpdatedEvent(
                    domainEvent.ProductId,
                    domainEvent.ProductName,
                    0,
                    0,
                    ECommerce.Domain.Models.InventoryOperationType.Lock,
                    "Stock locked",
                    domainEvent.OrderId));

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "StockLockedEventHandler: Error handling stock lock for product {ProductId}, order {OrderId}", domainEvent.ProductId, domainEvent.OrderId);
                return false;
            }
        }
    }
}


