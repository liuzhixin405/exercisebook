using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Architecture.Domain.Orders
{
    public class MissingOrder:Order
    {
        public static MissingOrder Instance = new MissingOrder();
        public MissingOrder()
        {

        }
    }
}
