using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using CodeMan.Seckill.Base.RabbitMq;
using CodeMan.Seckill.Base.RabbitMq.Config;
using CodeMan.Seckill.Base.RabbitMq.Message;
using CodeMan.Seckill.Service.service;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CodeMan.Seckill.Consumer.Pay
{
    public class ProcessPay : IHostedService
    {
        internal bool Started = false;
        private readonly RabbitConnection _connection;
        private readonly IPayService _payService;

        public ProcessPay(RabbitConnection connection, IPayService payService)
        {
            _connection = connection;
            _payService = payService;
            Queues.Add(new QueueInfo()
            {
                ExchangeType = ExchangeType.Direct,
                Exchange = RabbitConstant.DELAY_EXCHANGE,
                Queue = RabbitConstant.DELAY_QUEUE,
                RoutingKey = RabbitConstant.DELAY_ROUTING_KEY,
                props = new Dictionary<string, object>()
                {
                    {"x-dead-letter-exchange", RabbitConstant.DEAD_LETTER_EXCHANGE},
                    {"x-dead-letter-routing-key", RabbitConstant.DEAD_LETTER_ROUTING_KEY}
                },
                OnReceived = this.Receive
            });
        }

        public async void Receive(RabbitMessageEntity message)
        {
            Console.WriteLine($"Pay Receive Message:{message.Content}");
            OrderMessage orderMessage = JsonConvert.DeserializeObject<OrderMessage>(message.Content);

            // 超时未支付
            string many = "";
            // 支付处理
            Console.WriteLine("请输入:");
            // 超时未支付进行处理
            Task.Factory.StartNew(() =>
            {
                many = Console.ReadLine();
                //Console.WriteLine($"many:{many}");
            }).Wait(10 * 1000);
            if (string.Equals(many, "100"))
            {
                orderMessage.OrderInfo.Status = 1;
                await Task.Run(() => _payService.UpdateOrderPayState(orderMessage.OrderInfo));
                // _payService.UpdateOrderPayState(orderMessage.OrderInfo);
                Console.WriteLine("支付完成");
                message.Consumer.Model.BasicAck(message.BasicDeliver.DeliveryTag, true);
            }
            else
            {
                //重试几次依然失败
                Console.WriteLine("等待一定时间内失效超时未支付的订单");
                message.Consumer.Model.BasicNack(message.BasicDeliver.DeliveryTag, false, false);
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("RabbitMQ支付通知处理服务已启动");
            RabbitChannelManager channelManager = new RabbitChannelManager(_connection);
            foreach (var queueInfo in Queues)
            {
                RabbitChannelConfig channel = channelManager.CreateReceiveChannel(queueInfo.ExchangeType,
                    queueInfo.Exchange, queueInfo.Queue, queueInfo.RoutingKey, queueInfo.props);
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