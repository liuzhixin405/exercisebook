using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace designdemo
{
    internal interface IWeChatPay<T> :IPayment<T> where T:IPaymentEntity
    {
        void ConnectWeChatPay();
        void DisconnectWeChatPay();
    }
}
