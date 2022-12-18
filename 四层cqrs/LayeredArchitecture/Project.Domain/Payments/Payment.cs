using Project.Domain.Customers.Orders;
using Project.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.Payments
{
    public class Payment:Entity,IAggregateRoot
    {
        public PaymentId Id { get; private set; }
        private OrderId _orderId;
        private DateTime _createTime;
        private PaymentStatus _status;
        private bool _emailNotificationIsSent;
        private Payment() { }
        public Payment(OrderId orderId)
        {
            this.Id=new PaymentId(Guid.NewGuid());
            this._createTime = DateTime.UtcNow;
            this._orderId = orderId;
            this._status = PaymentStatus.ToPay;
            this._emailNotificationIsSent = false;
            this.AddDomainEvent(new PaymentCreatedEvent(this.Id, this._orderId));
        }
        public void MarkEmailNotificationIsSent()
        {
            this._emailNotificationIsSent=true;
        }
    }
}
