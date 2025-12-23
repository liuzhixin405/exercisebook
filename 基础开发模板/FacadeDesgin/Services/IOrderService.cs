namespace FacadeDesgin.Services;

using FacadeDesgin.Models;

public interface IOrderService
{
    Task<OrderDto> CreateOrderAsync(CreateOrderRequest request);
    Task<OrderDto?> GetOrderAsync(Guid id);
}
