using Project.Domain.SeedWork;
using Project.Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.Customers.Orders.Events
{
    public class OrderPlacedEvent:DomainEventBase
    {
        public OrderId OrderId { get; }
        public CustomerId CustomerId { get; }
        public MoneyValue Value { get; }
        public OrderPlacedEvent(OrderId orderId,CustomerId customerId,MoneyValue moneyValue)
        {
            this.OrderId = orderId;
            this.CustomerId = customerId;
            this.Value= moneyValue; 
        }
    }
}
