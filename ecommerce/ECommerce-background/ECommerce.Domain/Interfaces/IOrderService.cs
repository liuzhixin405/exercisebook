using ECommerce.Domain.Models;

namespace ECommerce.Domain.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<IEnumerable<OrderDto>> GetUserOrdersAsync(Guid userId);
        Task<OrderDto?> GetOrderByIdAsync(Guid id);
        Task<OrderDto> CreateOrderAsync(Guid userId, CreateOrderDto createOrderDto);
        Task<OrderDto> UpdateOrderStatusAsync(Guid id, UpdateOrderStatusDto updateOrderStatusDto);
        Task<bool> CancelOrderAsync(Guid id);
        Task<bool> ProcessPaymentAsync(PaymentDto paymentDto);
        // 在支付网关已成功后仅做订单侧的确认与事件发布（不再调用支付网关）
        Task<bool> FinalizePaymentAsync(Guid orderId, string paymentMethod, decimal amount, string paymentId);
        Task<bool> ConfirmOrderAsync(Guid orderId);
        Task<bool> ShipOrderAsync(Guid id, string trackingNumber);
        Task<bool> DeliverOrderAsync(Guid id);
        Task<bool> CompleteOrderAsync(Guid id);
        Task<bool> CancelExpiredOrdersAsync();
    }
}
