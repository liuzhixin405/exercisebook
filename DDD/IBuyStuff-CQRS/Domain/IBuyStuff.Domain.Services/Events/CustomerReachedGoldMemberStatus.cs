using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBuyStuff.Domain.Customers;

namespace IBuyStuff.Domain.Services.Events
{
    public class CustomerReachedGoldMemberStatus:IDomainEvent
    {
        public Customer Customer { get; set; }
    }
}
