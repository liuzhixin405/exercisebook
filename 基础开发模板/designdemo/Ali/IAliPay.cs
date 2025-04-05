using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace designdemo
{
    internal interface IAliPay<T> :IPayment<T> where T:IPaymentEntity
    {
        void ConnectAliPay();

        void DisconnectAliPay();
    }
}
