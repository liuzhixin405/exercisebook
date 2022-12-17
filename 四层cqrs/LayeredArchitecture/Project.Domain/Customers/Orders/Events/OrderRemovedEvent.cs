using Project.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.Customers.Orders.Events
{
    public class OrderRemovedEvent:DomainEventBase
    {
        public OrderId OrderId { get; }
        public OrderRemovedEvent(OrderId orderId)
        {
            this.OrderId = orderId;
        }
    }
}
