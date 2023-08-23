using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBuyStuff.Domain.Customers
{
    public class MissingCustomer : Customer
    {
        public static MissingCustomer Instance = new MissingCustomer();
    }
}
