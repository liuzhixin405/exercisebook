using Project.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.Customers
{
    public class CustomerRegisteredEvent:DomainEventBase
    {
        public CustomerId CustomerId { get; }
        public CustomerRegisteredEvent(CustomerId customerId)
        {
            this.CustomerId= customerId;
        }
    }
}
