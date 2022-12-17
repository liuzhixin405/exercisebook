using Project.Domain.Customers.Orders;
using Project.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.Payments
{
    public class PaymentCreateEvent:DomainEventBase
    {
        public PaymentCreateEvent(PaymentId paymentId,OrderId orderId)
        {
            this.PaymentId = paymentId;
            this.OrderId = orderId;
        }

        public PaymentId PaymentId { get; set; }
        public OrderId OrderId { get; set; }
    }
}
