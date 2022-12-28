using CodeMan.Seckill.Entities.Models;

using System;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Hosting;
using MimeKit;
using MimeKit.Text;

namespace CodeMan.Seckill.Consumer.Email
{
    public class EmailMessageSend
    {
        private EmailOption _option;

        public EmailMessageSend(EmailOption option)
        {
            _option = option;
        }

        public void Send(OrderInfo orderInfo, Account account)
        {
            // 使用哪种策略发送邮件在这里处理
            Console.WriteLine("发送邮件处理");
            EmailMessage emailMessage = new EmailMessage();
            emailMessage.Sender = new MailboxAddress("kevin", _option.Sender);
            emailMessage.Receiver = new MailboxAddress(account.Username, account.Email);
            emailMessage.Subject = "CodeMan商城-秒杀邮件提醒";
            emailMessage.Content = $"恭喜，秒杀到了{orderInfo.GoodsName}，订单号:{orderInfo.OrderId}";
            var mimeMessage = CreateEmailMessage(emailMessage);

            Console.WriteLine($"发送者：{_option.Sender}, 接收者邮件地址：{account.Email}, " +
                              $"邮件标题:{emailMessage.Subject}, 邮件内容：{emailMessage.Content}");

            using (SmtpClient client = new SmtpClient())
            {
                //Smtp服务器
                client.Connect(_option.SmtpServer, _option.Port, false);
                //登录，发送
                //特别说明，对于服务器端的中文相应，Exception中有编码问题，显示乱码了
                client.Authenticate(_option.Username, _option.Password);

                client.Send(mimeMessage);
                //断开
                client.Disconnect(true);
                Console.WriteLine("发送邮件成功");
            }

        }

        private MimeMessage CreateEmailMessage(EmailMessage message)
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(message.Sender);
            mimeMessage.To.Add(message.Receiver);
            mimeMessage.Subject = message.Subject;
            mimeMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text)
                { Text = message.Content };
            return mimeMessage;
        }
    }
}