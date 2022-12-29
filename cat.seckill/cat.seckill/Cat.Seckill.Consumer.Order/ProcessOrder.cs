using Cat.Seckill.Base.EFCore.Service;
using Cat.Seckill.Base.RabbitMq;
using Cat.Seckill.Base.RabbitMq.Config;
using Cat.Seckill.Base.RabbitMq.Message;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Cat.Seckill.Consumer.Order
{
    public class ProcessOrder : IHostedService
    { 
        internal bool Started = false;
        private RabbitConnection _connection;
        private readonly IServiceScopeFactory factory;
        public List<RabbitChannelConfig> Channels { get; set; }=new List<RabbitChannelConfig>();
        public List<QueueInfo> Queues { get; } = new List<QueueInfo>();
        
        public ProcessOrder(RabbitConnection connection, IServiceScopeFactory factory)
        {
            _connection= connection;
            this.factory = factory;
            Queues.Add(new QueueInfo
            {
                ExchangeType = ExchangeType.Fanout,
                Queue = RabbitConstant.SECKILL_QUEUE,
                RoutingKey = "",
                Exchange = RabbitConstant.SECKILL_EXCHANGE,
                OnReceived = this.Receive
            });
        }

        private async void Receive(RabbitMessageEntity message)
        {
            try
            {
                Console.WriteLine($"Order receive message:{message.Content}");
                var seckillMessage = System.Text.Json.JsonSerializer.Deserialize<SeckillMessage>(message.Content);
                await Task.Run(async () =>
                {
                    using var scope = factory.CreateAsyncScope();
                    var _seckillService = scope.ServiceProvider.GetRequiredService<ISeckillService>();
                    await _seckillService.SeckillOrder(seckillMessage);
                });
                message.Consumer.Model.BasicAck(message.BasicDeliver.DeliveryTag, true);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

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
