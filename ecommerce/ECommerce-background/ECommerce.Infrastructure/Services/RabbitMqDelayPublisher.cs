using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;

public class RabbitMqDelayPublisher : IRabbitMqDelayPublisher
{
    private readonly string _rabbitHost;
    private readonly string _rabbitUser;
    private readonly string _rabbitPass;
    private const string DelayQueueName = "order.delay.queue";
    private const string DeadLetterExchange = "order.dlx.exchange";
    private const string ExpiredQueueName = "order.expired.queue";

    public RabbitMqDelayPublisher(IConfiguration configuration)
    {
        _rabbitHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
        _rabbitUser = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "guest";
        _rabbitPass = Environment.GetEnvironmentVariable("RABBITMQ_PASS") ?? "guest";
    }

    public Task PublishOrderDelayMessageAsync(Guid orderId, TimeSpan delay)
    {
        var factory = new ConnectionFactory()
        {
            HostName = _rabbitHost,
            UserName = _rabbitUser,
            Password = _rabbitPass,
            DispatchConsumersAsync = true
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        // 声明交换机和队列
        channel.ExchangeDeclare(exchange: DeadLetterExchange, type: ExchangeType.Direct, durable: true, autoDelete: false, arguments: null);
        channel.QueueDeclare(queue: ExpiredQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        channel.QueueBind(queue: ExpiredQueueName, exchange: DeadLetterExchange, routingKey: ExpiredQueueName);

        var delayQueueArgs = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", DeadLetterExchange },
            { "x-dead-letter-routing-key", ExpiredQueueName }
        };
        channel.QueueDeclare(queue: DelayQueueName, durable: true, exclusive: false, autoDelete: false, arguments: delayQueueArgs);

        var body = Encoding.UTF8.GetBytes(orderId.ToString());
        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.Expiration = ((int)delay.TotalMilliseconds).ToString();

        channel.BasicPublish(exchange: "", routingKey: DelayQueueName, basicProperties: properties, body: body);

        return Task.CompletedTask;
    }
}
