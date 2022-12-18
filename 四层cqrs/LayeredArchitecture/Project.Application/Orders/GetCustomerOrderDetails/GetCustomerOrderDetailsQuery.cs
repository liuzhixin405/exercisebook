using Project.Application.Configuration.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Application.Orders.GetCustomerOrderDetails
{
    public class GetCustomerOrderDetailsQuery:IQuery<OrderDetailsDto>
    {
        public Guid OrderId { get; }

        public GetCustomerOrderDetailsQuery(Guid orderId)
        {
            this.OrderId = orderId;
        }
    }
}
