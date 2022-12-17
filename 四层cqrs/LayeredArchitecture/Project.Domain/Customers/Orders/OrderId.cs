using Project.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.Customers.Orders
{
    public class OrderId:TypedIdValueBase
    {
        public OrderId(Guid value):base(value)
        {

        }
    }
}
