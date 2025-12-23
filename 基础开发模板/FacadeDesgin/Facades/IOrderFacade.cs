namespace FacadeDesgin.Facades;

using FacadeDesgin.Models;

public interface IOrderFacade
{
    Task<OrderDto> PlaceOrderAsync(CreateOrderRequest request);
    Task<OrderDto?> FetchOrderAsync(Guid id);
}
