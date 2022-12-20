using Architecture.Domain.Customers;
using Architecture.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Architecture.Domain.Orders
{
    public class Order:IAggregateRoot
    {
     

        protected Order(int id, Customer customer)
        {
            this.OrderId = id;
            this.Buyer = customer;

            Total = Money.Zero;
            Items =new Collection<OrderItem>();
            State = OrderState.Pending;
            Date = DateTime.Today;
            
        }
        protected Order()
        {
            State = OrderState.Pending;
            Total = Money.Zero;
            Date = DateTime.Today;
            Items = new Collection<OrderItem>();
            Buyer = MissingCustomer.Instance;
        }


        public static Order CreateNew(int id,Customer customer)
        {
            var order = new Order(id,customer);
            return order;
        }
        public static Order CreateFromShoppingCart(int temporaryId,ShoppingCart cart)
        {
            var order = new Order(temporaryId, cart.Buyer);
            foreach (var item in cart.Items)
            {
                var orderitem = OrderItem.CreateNewForOrder(order, item.Quantity, item.Product);
                order.Items.Add(orderitem);
                order.Total = cart.GetTotal();
            }
            return order;
        }
        public int OrderId { get; private set; }
        public Customer Buyer { get; private set; }
        public ICollection<OrderItem> Items { get; private set; }
        public Money Total { get; private set; }
        public OrderState State { get; private set; }
        public DateTime Date { get; private set; }

        #region Behavior
        /// <summary>
        /// Add a collection of order items to the order
        /// </summary>
        /// <param name="items">Collection of order items</param>
        /// <returns>Same instance</returns>
        public Order AddItems(ICollection<OrderItem> items)
        {
            foreach (var item in items)
            {
                Items.Add(item);
            }
            return this;
        }

        public Order Cancel()
        {
            if (State != OrderState.Pending)
                throw new InvalidOperationException("Can't cancel an order that is not pending.");

            State = OrderState.Canceled;
            return this;
        }
        public Order Archive()
        {
            if (State != OrderState.Shipped)
                throw new InvalidOperationException("Can't archive an order that has not shipped yet.");

            State = OrderState.Canceled;
            return this;
        }
        public Order HasShipped()
        {
            if (State != OrderState.Shipped)
                throw new InvalidOperationException("Can't mark as shipped an order that is not pending.");

            State = OrderState.Shipped;
            return this;
        }
        #endregion

        #region Identity Management
        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;
            if (obj == null || GetType() != obj.GetType())
                return false;
            var other = (Order)obj;

            // Your identity logic goes here.  
            // You may refactor this code to the method of an entity interface 
            return OrderId == other.OrderId;
        }

        public override int GetHashCode()
        {
            return OrderId.GetHashCode();
        }
        #endregion
    }
}
