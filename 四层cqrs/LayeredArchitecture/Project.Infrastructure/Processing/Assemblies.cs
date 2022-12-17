using Project.Application.Orders.PlaceCustomerOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.Processing
{
    public class Assemblies
    {
        public static readonly Assembly Application = typeof(PlaceCustomerOrderCommand).Assembly;
    }
}
