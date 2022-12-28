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

namespace CodeMan.Seckill.Consumer.Order
{
    public class ProcessOrder : IHostedService
    {

        internal bool Started = false;
        private RabbitConnection _connection;
        private readonly ISeckillService _seckillService;

        public ProcessOrder(RabbitConnection connection, ISeckillService seckillService)
        {
            _seckillService = seckillService;
            _connection = connection;
            Queues.Add(new QueueInfo()
            {
                ExchangeType = ExchangeType.Fanout,
                Queue = RabbitConstant.SECKILL_QUEUE,
                RoutingKey = "",
                Exchange = RabbitConstant.SECKILL_EXCHANGE,
                OnReceived = this.Receive
            });
        }

        public async void Receive(RabbitMessageEntity message)
        {
            try
            {
                Console.WriteLine($"Order receive message:{message.Content}");
                var seckillMessage = JsonConvert.DeserializeObject<SeckillMessage>(message.Content);
                await Task.Run(() => _seckillService.Seckill(seckillMessage));
                //_seckillService.Seckill(seckillMessage);
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
            Console.WriteLine("RabbitMQ订单消息处理服务已启动");
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