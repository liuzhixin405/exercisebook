using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace designdemo
{
    internal class AliPayImpl : IAliPay<AliPayPaymentEntity>
    {
        public void ConnectAliPay()
        {
            Console.WriteLine("连接支付宝支付...");
        }


        public void DisconnectAliPay()
        {
            Console.WriteLine("断开支付宝支付连接...");
        }

        public void Pay(AliPayPaymentEntity payment)
        {
            Console.WriteLine($"支付宝支付：{payment.Amount}元，交易号：{payment.TransactionId}");
        }
    }
}
