using System;
using CodeMan.Seckill.Base.RabbitMq;
using CodeMan.Seckill.Base.RabbitMq.Message;
using CodeMan.Seckill.Entities.Models;

namespace CodeMan.Seckill.Consumer.Sms
{
    public class SmsMessage
    {
        public void Send(OrderMessage orderMessage)
        {
            // 使用哪种策略发送短信在这里处理
            Console.WriteLine($"发送短信处理,手机号:{orderMessage.Account.Phone}");
        }
    }
}