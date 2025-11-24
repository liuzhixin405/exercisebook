namespace Gitbitex.Spot.Domain.Matching;

public class Product
{
    public required string Id { get; set; }
    public int BaseScale { get; set; }
}

public class Order
{
    public long Id { get; set; }
    public required string ProductId { get; set; }
    public Side Side { get; set; }
    public OrderType Type { get; set; }
    public decimal Size { get; set; }
    public decimal Funds { get; set; }
    public decimal Price { get; set; }
    public string? Status { get; set; }
}

public sealed class BookOrder
{
    public long OrderId { get; set; }
    public decimal Size { get; set; }
    public decimal Funds { get; set; }
    public decimal Price { get; set; }
    public Side Side { get; set; }
    public OrderType Type { get; set; }

    public static BookOrder From(Order order) => new()
    {
        OrderId = order.Id,
        Size = order.Size,
        Funds = order.Funds,
        Price = order.Price,
        Side = order.Side,
        Type = order.Type
    };
}
