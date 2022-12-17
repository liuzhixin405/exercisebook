using Project.Domain.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.Customers.Orders
{
    public class OrderProductData
    {
        public OrderProductData(ProductId productId,int quantity)
        {
            ProductId = productId;
            Quantity= quantity;
        }
        public ProductId ProductId { get; }
        public int Quantity { get; }    
    }
}
