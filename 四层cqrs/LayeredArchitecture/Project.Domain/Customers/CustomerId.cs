using Project.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.Customers
{
    public class CustomerId:TypedIdValueBase
    {
        public CustomerId(Guid value):base(value)
        {

        }
    }
}
