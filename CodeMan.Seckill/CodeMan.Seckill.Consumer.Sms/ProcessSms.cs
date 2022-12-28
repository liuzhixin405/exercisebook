using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CodeMan.Seckill.Base.RabbitMq;
using CodeMan.Seckill.Base.RabbitMq.Config;
using CodeMan.Seckill.Base.RabbitMq.Message;
using CodeMan.Seckill.Entities.Models;

using Microsoft.Extensions.Hosting;

using Newtonsoft.Json;

using RabbitMQ.Client;

namespace CodeMan.Seckill.Consumer.Sms
{
    public class ProcessSms : IHostedService
    {
        internal bool Started = false;
        private readonly RabbitConnection _connection;
        private readonly SmsMessage _smsMessage;

        public ProcessSms(RabbitConnection connection, SmsMessage smsMessage)
        {
            _connection = connection;
            _smsMessage = smsMessage;
            Queues.Add(new QueueInfo()
            {
                ExchangeType = ExchangeType.Fanout,
                Queue = RabbitConstant.SMS_QUEUE,
                RoutingKey = "",
                Exchange = RabbitConstant.SMS_EXCHANGE,
                OnReceived = this.Receive
            });
        }

        public async void Receive(RabbitMessageEntity message)
        {
            try
            {
                Console.WriteLine($"SMS receive message:{message.Content}");
                var orderMessage = JsonConvert.DeserializeObject<OrderMessage>(message.Content);
                await Task.Run(() => _smsMessage.Send(orderMessage));
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
            Console.WriteLine("RabbitMQ短信通知处理服务已启动");
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