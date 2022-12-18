using Project.Application.Configuration.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Application.Customers.GetCustomerDetails
{
    public class GetCustomerDetailsQuery:IQuery<CustomerDetailsDto>
    {
        public GetCustomerDetailsQuery(Guid customerId)
        {
            CustomerId = customerId;
        }

        public Guid CustomerId { get; }
    }
}
