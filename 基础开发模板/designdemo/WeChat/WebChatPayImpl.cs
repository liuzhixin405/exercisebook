using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace designdemo
{
    internal class WeChatPayImpl : IWeChatPay<WeChatPaymentEntity>
    {
        public void ConnectWeChatPay()
        {
            Console.WriteLine("连接微信支付...");
        }

        public void DisconnectWeChatPay()
        {
            Console.WriteLine("断开微信支付连接...");
        }

        public void Pay(WeChatPaymentEntity payment)
        {
            Console.WriteLine($"微信支付：{payment.Amount}元，交易号：{payment.TransactionId}");
        }
    }
}
