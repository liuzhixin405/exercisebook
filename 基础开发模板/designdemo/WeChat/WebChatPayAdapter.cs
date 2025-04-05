using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace designdemo
{
    internal class WeChatPayAdapter<T>:IPayment<T> where T: IPaymentEntity
    {
        private readonly IWeChatPay<T> _WeChatPay;
        public WeChatPayAdapter(IWeChatPay<T> WeChatPay)
        {
            _WeChatPay = WeChatPay;
        }
        public void Pay(T payment)
        {
            _WeChatPay.ConnectWeChatPay();
            _WeChatPay.Pay(payment);
            _WeChatPay.DisconnectWeChatPay();
        }
    }
}
