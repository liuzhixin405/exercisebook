using Project.Application.Configuration.DomainEvents;
using Project.Domain.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Project.Application.Payments
{
    public class PaymentCreatedNotification:DomainNotificationBase<PaymentCreatedEvent>
    {
        public PaymentId PaymentId { get; }

        public PaymentCreatedNotification(PaymentCreatedEvent domainEvent) : base(domainEvent)
        {
            this.PaymentId = domainEvent.PaymentId;
        }
        [JsonConstructor]
        public PaymentCreatedNotification(PaymentId paymentId):base(null)
        {
            PaymentId= paymentId;
        }
    }
}
