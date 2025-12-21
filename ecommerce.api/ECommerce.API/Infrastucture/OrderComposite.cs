using ECommerce.API.Application;

namespace ECommerce.API.Infrastucture
{
    // 订单组合
    public class OrderComposite : IOrderComponent
    {
        private readonly List<IOrderComponent> _components = new();

        public void AddComponent(IOrderComponent component) => _components.Add(component);
        public void RemoveComponent(IOrderComponent component) => _components.Remove(component);

        public decimal GetTotal() => _components.Sum(c => c.GetTotal());

        public void Display(int indent = 0)
        {
            Console.WriteLine(new string(' ', indent) + "Order Summary:");
            foreach (var component in _components)
            {
                component.Display(indent + 2);
            }
            Console.WriteLine(new string(' ', indent) + $"Total: ${GetTotal():F2}");
        }
    }
}
