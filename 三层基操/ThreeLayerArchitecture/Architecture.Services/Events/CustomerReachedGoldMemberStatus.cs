using Architecture.Domain.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Architecture.Services.Events
{
    public class CustomerReachedGoldMemberStatus:IDomainEvent
    {
        public Customer Customer { get; set; }
    }
}
