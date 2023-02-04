using crypto_exchange_engine_simple.Engine;

var sellOrders = new List<Order>();
var buyerOrders = new List<Order>();
for (int i = 0; i < 100; i++)
{
    sellOrders.Add(new Order(Random.Shared.Next(1, 100), Random.Shared.Next(1, 10), (i+9).ToString(), Random.Shared.Next(0, 1)));
    buyerOrders.Add(new Order(Random.Shared.Next(1, 100), Random.Shared.Next(1, 10), (i + 110).ToString(), Random.Shared.Next(0, 1)));
}

var orderBook = new OrderBook(sellOrders.OrderByDescending(x=>x.Price).ToArray(),buyerOrders.OrderBy(x=>x.Price).ToArray());

//price顺序影响结果

Console.WriteLine("交易开始"); 
for (int i = 9; i < 20; i++)
{
    var order = new Order(Random.Shared.Next(1, 10), Random.Shared.Next(1, 10), i.ToString(), Random.Shared.Next(0, 1));
    Console.WriteLine($"挂单:{order.ToString()}");
    var result = await orderBook.Process(order);
    Console.WriteLine("==========================================");
    foreach (var item in result)
    {
        Console.WriteLine($"交易信息:{item.ToString()}");
    }
    Console.WriteLine("==========================================");
    Console.WriteLine();
}


Console.Read();
