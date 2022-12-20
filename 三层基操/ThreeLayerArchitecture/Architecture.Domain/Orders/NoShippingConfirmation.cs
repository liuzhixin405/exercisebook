using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Architecture.Domain.Orders
{
    public class NoShippingConfirmation:OrderShippingConfirmation
    {
        public static NoShippingConfirmation Instance = new NoShippingConfirmation();
        private NoShippingConfirmation()
        {
        }
    }
}
