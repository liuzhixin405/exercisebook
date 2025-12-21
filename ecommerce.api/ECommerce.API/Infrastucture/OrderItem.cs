using ECommerce.API.Application;

namespace ECommerce.API.Infrastucture
{
    // 单个订单项
    public class OrderItem : IOrderComponent
    {
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public decimal GetTotal() => Price * Quantity;

        public void Display(int indent = 0)
        {
            Console.WriteLine(new string(' ', indent) + $"- {ProductName}: {Quantity} x ${Price}");
        }
    }
}
