using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace designdemo
{
    internal interface IPaymentEntity
    {
        decimal Amount { get; set; }
        string TransactionId { get; set; }
    }
}
