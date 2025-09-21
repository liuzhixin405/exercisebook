using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

public class RabbitMqConnectionProvider : IRabbitMqConnectionProvider
{
    private readonly string _rabbitHost;
    private readonly string _rabbitUser;
    private readonly string _rabbitPass;
    private IConnection? _connection;
    private IModel? _channel;

    private const string DeadLetterExchange = "order.dlx.exchange";
    private const string ExpiredQueueName = "order.expired.queue";
    private const string DelayQueueName = "order.delay.queue";
    private const string ConfirmationQueueName = "order.confirmation.queue";
    private const string ShipmentQueueName = "order.shipment.queue";
    private const string CompletionQueueName = "order.completion.queue";

    public RabbitMqConnectionProvider(IConfiguration configuration)
    {
        // 优先从配置文件读取，然后从环境变量读取，最后使用默认值
        _rabbitHost = configuration["RabbitMQ:Host"] ?? 
                     Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? 
                     "localhost";
        _rabbitUser = configuration["RabbitMQ:Username"] ?? 
                     Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? 
                     "guest";
        _rabbitPass = configuration["RabbitMQ:Password"] ?? 
                     Environment.GetEnvironmentVariable("RABBITMQ_PASS") ?? 
                     "guest";
    }

    public IConnection GetConnection()
    {
        if (_connection == null || !_connection.IsOpen)
        {
            var factory = new ConnectionFactory
            {
                HostName = _rabbitHost,
                UserName = _rabbitUser,
                Password = _rabbitPass,
                DispatchConsumersAsync = true
            };
            _connection = factory.CreateConnection();
        }
        return _connection;
    }

    public IModel GetChannel()
    {
        if (_channel == null || _channel.IsClosed)
        {
            _channel = GetConnection().CreateModel();
        }
        return _channel;
    }

    public void EnsureQueuesAndExchanges()
    {
        try
        {
            var channel = GetChannel();
            channel.ExchangeDeclare(exchange: DeadLetterExchange, type: ExchangeType.Direct, durable: true, autoDelete: false, arguments: null);
            channel.QueueDeclare(queue: ExpiredQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueBind(queue: ExpiredQueueName, exchange: DeadLetterExchange, routingKey: ExpiredQueueName);

            var delayQueueArgs = new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", DeadLetterExchange },
                { "x-dead-letter-routing-key", ExpiredQueueName }
            };
            channel.QueueDeclare(queue: DelayQueueName, durable: true, exclusive: false, autoDelete: false, arguments: delayQueueArgs);

            // 创建确认队列
            channel.QueueDeclare(queue: ConfirmationQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            
            // 创建发货队列
            channel.QueueDeclare(queue: ShipmentQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            
            // 创建完成队列
            channel.QueueDeclare(queue: CompletionQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        }
        catch (Exception ex)
        {
            // 记录错误但不抛出异常，避免阻止应用启动
            Console.WriteLine($"Warning: Failed to ensure RabbitMQ queues and exchanges: {ex.Message}");
        }
    }
}