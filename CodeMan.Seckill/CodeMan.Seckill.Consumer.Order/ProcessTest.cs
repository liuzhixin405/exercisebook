using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CodeMan.Seckill.Base.RabbitMq;
using CodeMan.Seckill.Base.RabbitMq.Config;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;

namespace CodeMan.Seckill.Consumer.Order
{
    public class ProcessTest : IHostedService
    {
        //private readonly ISeckillService _seckillService;
        private RabbitConnection _connection;
        public ProcessTest(RabbitConnection connection)
        {
            _connection = connection;
            Queues.Add(new QueueInfo()
            {
                ExchangeType = ExchangeType.Fanout,
                Queue = RabbitConstant.TEST_QUEUE,
                RoutingKey = "",
                Exchange = RabbitConstant.TEST_EXCHANGE,
                OnReceived = this.Receive
            });
        }

        public void Receive(RabbitMessageEntity message)
        {
            try
            {
                Console.WriteLine($"receive message:{message.Content}");
                message.Consumer.Model.BasicAck(message.BasicDeliver.DeliveryTag, true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public List<RabbitChannelConfig> Channels { get; set; } = new List<RabbitChannelConfig>();

        public List<QueueInfo> Queues { get; } = new List<QueueInfo>();

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("RabbitMQ测试消息处理服务已启动");
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
    }
}