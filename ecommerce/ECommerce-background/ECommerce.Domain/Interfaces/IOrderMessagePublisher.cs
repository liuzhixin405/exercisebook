namespace ECommerce.Domain.Interfaces
{
    public interface IOrderMessagePublisher
    {
        Task PublishOrderConfirmationMessageAsync(Guid orderId, Guid userId);
        Task PublishShipmentMessageAsync(Guid orderId, Guid userId);
        Task PublishCompletionMessageAsync(Guid orderId, Guid userId);
    }
}
