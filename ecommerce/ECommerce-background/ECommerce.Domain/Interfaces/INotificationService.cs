using ECommerce.Domain.Events;

namespace ECommerce.Domain.Interfaces
{
    public interface INotificationService
    {
        Task SendLowStockNotificationAsync(InventoryUpdatedEvent inventoryEvent);
        Task SendOrderStatusNotificationAsync(string orderId, string customerId, string status);
        Task SendPaymentNotificationAsync(PaymentProcessedEvent paymentEvent);
        Task SendInventoryAlertAsync(InventoryUpdatedEvent inventoryEvent);
        Task SendSystemAlertAsync(string message, string level);
        Task SendCustomerNotificationAsync(string customerId, string message, string type);
        Task SendInventoryOutNotificationAsync(InventoryUpdatedEvent inventoryEvent);
        Task SendInventoryInNotificationAsync(InventoryUpdatedEvent inventoryEvent);
        Task SendInventoryLockNotificationAsync(InventoryUpdatedEvent inventoryEvent);
        Task SendInventoryChangeNotificationAsync(InventoryUpdatedEvent inventoryEvent);
    }
}
