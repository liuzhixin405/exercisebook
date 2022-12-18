using Project.Application.Configuration.DomainEvents;
using Project.Domain.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Project.Application.Customers.IntegrationHandlers
{
    public class CustomerRegisteredNotification:DomainNotificationBase<CustomerRegisteredEvent>
    {
        public CustomerId CustomerId { get; }

        public CustomerRegisteredNotification(CustomerRegisteredEvent domainEvent) : base(domainEvent)
        {
            this.CustomerId = domainEvent.CustomerId;
        }

        [JsonConstructor]
        public CustomerRegisteredNotification(CustomerId customerId) : base(null)
        {
            this.CustomerId = customerId;
        }
    }
}
