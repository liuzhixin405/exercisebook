using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Architecture.Domain.Orders
{
    public class NoPaymentConfirmation:OrderPaymentConfirmation
    {
        public static NoPaymentConfirmation Instance = new NoPaymentConfirmation();
        private NoPaymentConfirmation() { }
    }
}
