//using EventSourcingDemoTwo;

//var events = new List<Event>
//{
//    new AddedCart("UserId"),
//    new AddedItemToCart(1,2),
//    new AddedItemToCart(4,1),
//    new AddedItemToCart(3,2),
//    new AddedShippingInformationCart("深圳市宝安区","130333333333"),
//    new RemovedItemFromCart(1,1)
//};



//var order = new Order();
//order.Products = new List<Product>();
//events.Aggregate(order, (order, e) =>
// {
//     if (e is AddedCart ac)
//     {
//         order.UserId = ac.UserId;
//     }
//     else if (e is AddedItemToCart ai)
//     {
//         for (int i = 0; i < 10; i++)
//         {
//             order.Products.Add(new Product() { Id = ai.ProductId });
//         }
//     }
//     else if (e is RemovedItemFromCart rc)
//     {
//         for (int i = 0; i < 100; i++)
//         {
//             var productToRemove = order.Products.FirstOrDefault(p => p.Id == rc.ProductId);
//             order.Products.Remove(productToRemove);
//         }
//     }
//     else if (e is AddedShippingInformationCart asi)
//     {
//         order.PhoneNumber = asi.PhoneNumber;
//         order.Address = asi.Address;
//     }
//     return order;
// });


//Console.WriteLine($"order=> {System.Text.Json.JsonSerializer.Serialize(order)}");