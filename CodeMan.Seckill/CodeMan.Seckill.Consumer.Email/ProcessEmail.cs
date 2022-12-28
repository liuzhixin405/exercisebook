using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CodeMan.Seckill.Base.RabbitMq;
using CodeMan.Seckill.Base.RabbitMq.Config;
using CodeMan.Seckill.Base.RabbitMq.Message;
using CodeMan.Seckill.Entities.Models;
using CodeMan.Seckill.Service.service;
using Microsoft.Extensions.Hosting;

using Newtonsoft.Json;

using RabbitMQ.Client;

namespace CodeMan.Seckill.Consumer.Email
{
    public class ProcessEmail : IHostedService
    {
        private readonly RabbitConnection _connection;
        private readonly EmailMessageSend _email;

        public ProcessEmail(RabbitConnection connection, EmailMessageSend email)
        {
            _connection = connection;
            _email = email;
            //_userService = userService;
            Queues.Add(new QueueInfo()
            {
                ExchangeType = ExchangeType.Fanout,
                Exchange = RabbitConstant.EMAIL_EXCHANGE,
                Queue = RabbitConstant.EMAIL_QUEUE,
                RoutingKey = "",
                OnReceived = this.Receive
            });
        }

        public async void Receive(RabbitMessageEntity message)
        {
            try
            {
                Console.WriteLine($"Email receive message:{message.Content}");
                var orderMessage = JsonConvert.DeserializeObject<OrderMessage>(message.Content);
                if (null != orderMessage)
                {
                    await Task.Run(() => _email.Send(orderMessage.OrderInfo, orderMessage.Account));
                    //_email.Send(orderMessage.OrderInfo, orderMessage.Account);
                }
                message.Consumer.Model.BasicAck(message.BasicDeliver.DeliveryTag, true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("RabbitMQ邮件通知消息处理服务已启动");
            RabbitChannelManager channelManager = new RabbitChannelManager(_connection);
            foreach (var queueInfo in Queues)
            {
                RabbitChannelConfig channel = channelManager.CreateReceiveChannel(queueInfo.ExchangeType,
                    queueInfo.Exchange, queueInfo.Queue, queueInfo.RoutingKey);
                channel.OnReceivedCallback = queueInfo.OnReceived;
                //this.Channels.Add(channel);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public List<RabbitChannelConfig> Channels { get; set; } = new List<RabbitChannelConfig>();

        public List<QueueInfo> Queues { get; } = new List<QueueInfo>();
    }
}