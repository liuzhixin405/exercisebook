using Architecture.Domain.Customers;
using Architecture.Domain.Products;
using Architecture.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Architecture.Domain.Orders
{
    public class ShoppingCart
    {
        private ShoppingCart()
        {
            Items = new List<ShoppingCartItem>();
        }
        public static ShoppingCart CreateEmpty(Customer buyer)
        {
            var model = new ShoppingCart() { Buyer = buyer };
            return model;
        }
        public ShoppingCart AddItem(int quantity, Product product)
        {
            var existingItem = (from i in Items where i.Product.Id == product.Id select i).FirstOrDefault();
            if (existingItem != null)
            {
                existingItem.Quantity++;
                return this;
            }
            Items.Add(ShoppingCartItem.Create(quantity, product));
            return this;
        }
        public Money GetTotal()
        {
            var total = (decimal)0;
            foreach (var item in Items)
            {
                total += item.Quantity * item.Product.UnitPrice.Value;
            }
            return new Money(Currency.Default, total);
        }
        public Customer Buyer { get; private set; }
        public IList<ShoppingCartItem> Items { get; private set; }
    }
}
