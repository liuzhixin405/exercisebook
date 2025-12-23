namespace FacadeDesgin.Facades;

using FacadeDesgin.Models;
using FacadeDesgin.Services;

public class OrderFacade : IOrderFacade
{
    private readonly IOrderService _orderService;
    private readonly IInventoryService _inventoryService;
    private readonly IPaymentService _paymentService;
    private readonly IEventPublisher _eventPublisher;
    private readonly INotificationService _notificationService;

 
    public OrderFacade(
        IOrderService orderService,
        IInventoryService inventoryService,
        IPaymentService paymentService,
        IEventPublisher eventPublisher,
        INotificationService notificationService)
    {
        _orderService = orderService;
        _inventoryService = inventoryService;
        _paymentService = paymentService;
        _eventPublisher = eventPublisher;
        _notificationService = notificationService;
    }

    public async Task<OrderDto> PlaceOrderAsync(CreateOrderRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CustomerName))
            throw new ArgumentException("CustomerName is required", nameof(request.CustomerName));

        // reserve inventory for all items
        var reserved = new List<OrderItem>();
        foreach (var item in request.Items)
        {
            var ok = await _inventoryService.ReserveAsync(item.Product, item.Quantity);
            if (!ok)
            {
                // release any already reserved
                foreach (var r in reserved)
                    await _inventoryService.ReleaseAsync(r.Product, r.Quantity);

                throw new InvalidOperationException($"Insufficient stock for {item.Product}");
            }

            reserved.Add(item);
        }

        var total = request.Items.Sum(i => i.Price * i.Quantity);

        // charge payment
        var (success, tx) = await _paymentService.ChargeAsync(request.Payment, total);
        if (!success)
        {
            // rollback reservation
            foreach (var r in reserved)
                await _inventoryService.ReleaseAsync(r.Product, r.Quantity);

            throw new InvalidOperationException("Payment failed");
        }

        // create order record
        var order = await _orderService.CreateOrderAsync(request);
        order.Items = request.Items.Select(i => new OrderItem { Product = i.Product, Price = i.Price, Quantity = i.Quantity }).ToList();
        order.PaymentStatus = PaymentStatus.Completed;
        order.TransactionId = tx;

        // publish event
        await _eventPublisher.PublishAsync("order.created", new { order.Id, order.Total, order.CustomerName });

        // send notifications via notification service (uses bridge senders internally)
        try
        {
            await _notificationService.SendAsync(request, "Order Confirmed", $"Your order {order.Id} has been confirmed.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Notification failed: {ex.Message}");
        }

        return order;
    }

    public Task<OrderDto?> FetchOrderAsync(Guid id)
        => _orderService.GetOrderAsync(id);
}
