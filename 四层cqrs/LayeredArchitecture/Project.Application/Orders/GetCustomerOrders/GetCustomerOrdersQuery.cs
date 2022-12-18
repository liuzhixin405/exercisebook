using Project.Application.Configuration.Queries;
using Project.Domain.Customers.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Application.Orders.GetCustomerOrders
{
    public class GetCustomerOrdersQuery:IQuery<List<OrderDto>>
    {
        public Guid CustomerId { get; }

        public GetCustomerOrdersQuery(Guid customerId)
        {
            this.CustomerId = customerId;
        }
    }
}
