using Gitbitex.Spot.Domain.Matching;

Console.WriteLine("OrderBook in-memory demo");

var product = new Product
{
    Id = "BTC-USDT",
    BaseScale = 4
};

var book = new OrderBook(product);

var orders = new List<Order>
{
    new()
    {
        Id = 1,
        ProductId = product.Id,
        Side = Side.Sell,
        Type = OrderType.Limit,
        Price = 30000m,
        Size = 0.5m
    },
    new()
    {
        Id = 2,
        ProductId = product.Id,
        Side = Side.Buy,
        Type = OrderType.Limit,
        Price = 31000m,
        Size = 0.3m
    }
};

foreach (var order in orders)
{
    var logs = book.ApplyOrder(order);
    PrintLogs(logs);
}

Console.WriteLine("Done.");

static void PrintLogs(IReadOnlyList<LogBase> logs)
{
    foreach (var log in logs)
    {
        switch (log)
        {
            case OpenLog open:
                Console.WriteLine(
                    $"OPEN  seq={open.Seq} id={open.Order.OrderId} side={open.Order.Side} price={open.Order.Price} size={open.Order.Size}");
                break;
            case MatchLog match:
                Console.WriteLine(
                    $"MATCH seq={match.Seq} tradeSeq={match.TradeSeq} taker={match.Taker.OrderId} maker={match.Maker.OrderId} price={match.Price} size={match.Size}");
                break;
            case DoneLog done:
                Console.WriteLine(
                    $"DONE  seq={done.Seq} id={done.Order.OrderId} reason={done.Reason} remaining={done.RemainingSize}");
                break;
            default:
                Console.WriteLine($"LOG   {log}");
                break;
        }
    }
}