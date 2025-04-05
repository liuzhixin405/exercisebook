using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace designdemo
{
    internal class AliPayAdapter<T> : IPayment<T> where T : IPaymentEntity
    {
        private readonly IAliPay<T> _aliPay;
        public AliPayAdapter(IAliPay<T> aliPay)
        {
            _aliPay = aliPay;
        }
        public void Pay(T payment)
        {
            _aliPay.ConnectAliPay();
            _aliPay.Pay(payment);
            _aliPay.DisconnectAliPay();
        }
    
    }
}
