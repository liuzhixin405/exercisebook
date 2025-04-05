using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace designdemo
{
    internal class AliPayPaymentEntity : IPaymentEntity
    {
        public decimal Amount { get ; set ; }
        public string TransactionId { get; set ; }
    }
}
