using Project.Application.Orders;

namespace Project.API.Orders
{
    public class CustomerOrderRequest
    {
        public List<ProductDto> Products { get; set; }

        public string Currency { get; set; }
    }
}
