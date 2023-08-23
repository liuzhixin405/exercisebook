using IBuyStuff.Domain.Products;
using IBuyStuff.Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace IBuyStuff.Domain.Orders
{
    [Owned]
    public class OrderItem
    {
        public static OrderItem CreateNewForOrder(Order order, int quantity, Product product)
        {
            var item = new OrderItem { Quantity = quantity, ProductId = product.Id };
            return item;
        }

        protected OrderItem()
        {
        }

        public int Id { get; private set; }
        public int Quantity { get; set; }
        //public Product Product { get; set; }
        //public Order Order { get; private set; }
        public int ProductId { get; set; }
        #region Behavior
        /// <summary>
        /// Get the total of the order item
        /// </summary>
        /// <returns>Total</returns>
        //public Money GetTotal()
        //{
        //    return new Money(Product.UnitPrice.Currency, Quantity*Product.UnitPrice.Value);
        //}
        #endregion
    }
}