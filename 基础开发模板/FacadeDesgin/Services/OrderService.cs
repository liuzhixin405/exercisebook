namespace FacadeDesgin.Services;

using FacadeDesgin.Models;

public class OrderService : IOrderService
{
    private readonly Dictionary<Guid, OrderDto> _store = new();

    public Task<OrderDto> CreateOrderAsync(CreateOrderRequest request)
    {
        var total = request.Items.Sum(i => i.Price * i.Quantity);
        var order = new OrderDto
        {
            Id = Guid.NewGuid(),
            CustomerName = request.CustomerName,
            CreatedAt = DateTime.UtcNow,
            Total = total,
            Items = request.Items.Select(i => new OrderItem { Product = i.Product, Price = i.Price, Quantity = i.Quantity }).ToList(),
            PaymentStatus = PaymentStatus.Pending
        };

        _store[order.Id] = order;
        return Task.FromResult(order);
    }

    public Task<OrderDto?> GetOrderAsync(Guid id)
    {
        _store.TryGetValue(id, out var order);
        return Task.FromResult(order);
    }
}
