using ECommerce.Core.EventBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace ECommerce.Infrastructure.EventBus
{
    /// <summary>
    /// 基于RabbitMQ的分布式事件总线
    /// </summary>
    public class RabbitMQEventBus : IEventBus, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ILogger<RabbitMQEventBus> _logger;
        private readonly Dictionary<string, List<Type>> _handlers;
        private readonly IServiceProvider _serviceProvider;
        private readonly string _brokerName;
        private readonly string _queueName;

        public RabbitMQEventBus(
            IOptions<RabbitMQSettings> options,
            IServiceProvider serviceProvider,
            ILogger<RabbitMQEventBus> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _handlers = new Dictionary<string, List<Type>>();
            _brokerName = options.Value.BrokerName;
            _queueName = options.Value.QueueName;

            // 创建RabbitMQ连接
            var factory = new ConnectionFactory
            {
                HostName = options.Value.HostName,
                Port = options.Value.Port,
                UserName = options.Value.UserName,
                Password = options.Value.Password,
                VirtualHost = options.Value.VirtualHost
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // 声明交换机和队列
            DeclareExchangeAndQueue();
        }

        /// <summary>
        /// 发布事件到RabbitMQ
        /// </summary>
        public async Task PublishAsync<T>(T @event) where T : class
        {
            try
            {
                var eventName = @event.GetType().Name;
                var message = JsonConvert.SerializeObject(@event);
                var body = Encoding.UTF8.GetBytes(message);

                // 发布到交换机
                _channel.BasicPublish(
                    exchange: _brokerName,
                    routingKey: eventName,
                    basicProperties: null,
                    body: body);

                _logger.LogInformation("Event {EventName} published to RabbitMQ", eventName);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish event {EventType} to RabbitMQ", typeof(T).Name);
                throw;
            }
        }

        /// <summary>
        /// 订阅事件类型
        /// </summary>
        public void Subscribe<T, TH>() where T : class where TH : IEventHandler<T>
        {
            var eventName = typeof(T).Name;
            var handlerType = typeof(TH);

            if (!_handlers.ContainsKey(eventName))
            {
                _handlers[eventName] = new List<Type>();
            }

            if (_handlers[eventName].Contains(handlerType))
            {
                _logger.LogWarning("Handler {HandlerType} already registered for event {EventName}", handlerType.Name, eventName);
                return;
            }

            _handlers[eventName].Add(handlerType);

            // 绑定队列到交换机
            _channel.QueueBind(
                queue: _queueName,
                exchange: _brokerName,
                routingKey: eventName);

            _logger.LogInformation("Handler {HandlerType} subscribed to event {EventName}", handlerType.Name, eventName);
        }

        /// <summary>
        /// 取消订阅
        /// </summary>
        public void Unsubscribe<T, TH>() where T : class where TH : IEventHandler<T>
        {
            var eventName = typeof(T).Name;
            var handlerType = typeof(TH);

            if (_handlers.ContainsKey(eventName))
            {
                _handlers[eventName].Remove(handlerType);
                _logger.LogInformation("Handler {HandlerType} unsubscribed from event {EventName}", handlerType.Name, eventName);
            }
        }

        /// <summary>
        /// 启动事件消费
        /// </summary>
        public void StartConsuming()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += Consumer_Received;

            _channel.BasicConsume(
                queue: _queueName,
                autoAck: false,
                consumer: consumer);

            _logger.LogInformation("Event consumer started for queue {QueueName}", _queueName);
        }

        /// <summary>
        /// 事件消费处理
        /// </summary>
        private async void Consumer_Received(object? sender, BasicDeliverEventArgs e)
        {
            try
            {
                var eventName = e.RoutingKey;
                var message = Encoding.UTF8.GetString(e.Body.Span);

                _logger.LogInformation("Received event {EventName}: {Message}", eventName, message);

                if (_handlers.ContainsKey(eventName))
                {
                    var subscriptions = _handlers[eventName];
                    
                    foreach (var subscription in subscriptions)
                    {
                        // 通过依赖注入创建处理器实例
                        var handler = _serviceProvider.GetService(subscription);
                        if (handler != null)
                        {
                            // 反序列化事件
                            var eventType = Type.GetType(eventName);
                            if (eventType != null)
                            {
                                var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                                
                                // 调用处理器
                                var concreteType = typeof(IEventHandler<>).MakeGenericType(eventType);
                                var handleMethod = concreteType.GetMethod("HandleAsync");
                                
                                if (handleMethod != null)
                                {
                                    await (Task)handleMethod.Invoke(handler, new[] { integrationEvent })!;
                                }
                            }
                        }
                    }

                    // 确认消息已处理
                    _channel.BasicAck(e.DeliveryTag, false);
                }
                else
                {
                    _logger.LogWarning("No handlers found for event {EventName}", eventName);
                    // 拒绝消息，不重新入队
                    _channel.BasicNack(e.DeliveryTag, false, false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing event {EventName}", e.RoutingKey);
                // 拒绝消息，重新入队
                _channel.BasicNack(e.DeliveryTag, false, true);
            }
        }

        /// <summary>
        /// 声明交换机和队列
        /// </summary>
        private void DeclareExchangeAndQueue()
        {
            // 声明交换机
            _channel.ExchangeDeclare(
                exchange: _brokerName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false);

            // 声明队列
            _channel.QueueDeclare(
                queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false);

            _logger.LogInformation("Exchange {Exchange} and queue {Queue} declared", _brokerName, _queueName);
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }

    /// <summary>
    /// RabbitMQ配置
    /// </summary>
    public class RabbitMQSettings
    {
        public string HostName { get; set; } = "localhost";
        public int Port { get; set; } = 5672;
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string VirtualHost { get; set; } = "/";
        public string BrokerName { get; set; } = "ecommerce_event_bus";
        public string QueueName { get; set; } = "ecommerce_event_queue";
    }
}
