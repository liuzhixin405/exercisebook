using Architecture.Domain.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Architecture.Domain.Orders
{
    public class ShoppingCartItem
    {
        public static ShoppingCartItem Create(int quantity,Product product)
        {
            var item = new ShoppingCartItem { Quantity = quantity, Product = product};
            return item;
        }
        public ShoppingCartItem()
        {
            Quantity = 0;
            Product = new NullProduct();
        }
        public int Quantity { get; set; }
        public Product Product { get; set; }
    }
}
