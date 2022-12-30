using Cat.Seckill.Base.EFCore.Service;
using Cat.Seckill.Base.RabbitMq.Config;
using Cat.Seckill.Base.RabbitMq.Message;
using Cat.Seckill.Base.RabbitMq;
using RabbitMQ.Client;

namespace Cat.Seckill.Consumer.Pay
{
    public class ProcessPayTimeout : IHostedService
    {

        internal bool Started = false;
        private readonly RabbitConnection _connection;
       
        private readonly IServiceScopeFactory factory;
        public ProcessPayTimeout(RabbitConnection connection,  IServiceScopeFactory factory)
        {
            _connection = connection;
           
            Queues.Add(new QueueInfo()
            {
                ExchangeType = ExchangeType.Direct,
                Exchange = RabbitConstant.DEAD_LETTER_EXCHANGE,
                Queue = RabbitConstant.DEAD_LETTER_QUEUE,
                RoutingKey = RabbitConstant.DEAD_LETTER_ROUTING_KEY,
                OnReceived = this.Receive
            });
            this.factory = factory;
        }

        public async void Receive(RabbitMessageEntity message)
        {
            Console.WriteLine($"Pay Timeout Receive Message:{message.Content}");
            OrderMessage orderMessage = System.Text.Json.JsonSerializer.Deserialize<OrderMessage>(message.Content);
            orderMessage.OrderInfo.Status = 2;
            Console.WriteLine("超时未支付");
            // _payService.UpdateOrderPayState(orderMessage.OrderInfo);
            using var scope = factory.CreateAsyncScope();
            var _payService = scope.ServiceProvider.GetRequiredService<IPayService>();
            await Task.Run(() => _payService.UpdateOrderPayState(orderMessage.OrderInfo));
            message.Consumer.Model.BasicAck(message.BasicDeliver.DeliveryTag, true);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("RabbitMQ超时支付处理服务已启动");
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