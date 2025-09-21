using ECommerce.Domain.Events;

namespace ECommerce.Domain.Interfaces
{
    public interface IStatisticsService
    {
        Task UpdateOrderStatisticsAsync(OrderCreatedEvent orderEvent);
        Task UpdatePaymentStatisticsAsync(OrderPaidEvent orderEvent);
        Task UpdateCancellationStatisticsAsync(OrderCancelledEvent orderEvent);
        Task UpdateInventoryStatisticsAsync(InventoryUpdatedEvent inventoryEvent);
        Task UpdateUserActivityStatisticsAsync(string userId, string activityType);
        Task UpdateProductViewStatisticsAsync(int productId);
        Task UpdateSalesStatisticsAsync(OrderPaidEvent orderEvent);
    }
}
