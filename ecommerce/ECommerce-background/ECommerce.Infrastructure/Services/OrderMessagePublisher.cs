using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using ECommerce.Domain.Interfaces;

namespace ECommerce.Infrastructure.Services
{
    public class OrderMessagePublisher : IOrderMessagePublisher
    {
        private readonly string _rabbitHost;
        private readonly string _rabbitUser;
        private readonly string _rabbitPass;
        private const string ConfirmationQueueName = "order.confirmation.queue";
        private const string ShipmentQueueName = "order.shipment.queue";
        private const string CompletionQueueName = "order.completion.queue";

        public OrderMessagePublisher(IConfiguration configuration)
        {
            _rabbitHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
            _rabbitUser = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "guest";
            _rabbitPass = Environment.GetEnvironmentVariable("RABBITMQ_PASS") ?? "guest";
        }

        public Task PublishOrderConfirmationMessageAsync(Guid orderId, Guid userId)
        {
            try
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

                // 确保队列存在
                channel.QueueDeclare(queue: ConfirmationQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

                var confirmationData = new OrderConfirmationData
                {
                    OrderId = orderId,
                    UserId = userId,
                    RequestedAt = DateTime.UtcNow
                };

                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(confirmationData));
                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish(exchange: "", routingKey: ConfirmationQueueName, basicProperties: properties, body: body);
            }
            catch (Exception ex)
            {
                // 用 Console 输出兜底日志（发布端无 ILogger 注入）
                Console.WriteLine($"Error publishing order confirmation message: {ex.Message}");
                // 不中断主流程
            }

            return Task.CompletedTask;
        }

        public Task PublishShipmentMessageAsync(Guid orderId, Guid userId)
        {
            try
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

                // 确保队列存在
                channel.QueueDeclare(queue: ShipmentQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

                var shipmentData = new OrderShipmentData
                {
                    OrderId = orderId,
                    UserId = userId,
                    RequestedAt = DateTime.UtcNow
                };

                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(shipmentData));
                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish(exchange: "", routingKey: ShipmentQueueName, basicProperties: properties, body: body);
            }
            catch (Exception ex)
            {
                // 用 Console 输出兜底日志（发布端无 ILogger 注入）
                Console.WriteLine($"Error publishing shipment message: {ex.Message}");
                // 不中断主流程
            }

            return Task.CompletedTask;
        }

        public Task PublishCompletionMessageAsync(Guid orderId, Guid userId)
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

            // 确保队列存在
            channel.QueueDeclare(queue: CompletionQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var completionData = new OrderCompletionData
            {
                OrderId = orderId,
                UserId = userId,
                RequestedAt = DateTime.UtcNow
            };

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(completionData));
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(exchange: "", routingKey: CompletionQueueName, basicProperties: properties, body: body);

            return Task.CompletedTask;
        }
    }

    public class OrderConfirmationData
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public DateTime RequestedAt { get; set; }
    }

    public class OrderShipmentData
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public DateTime RequestedAt { get; set; }
    }

    public class OrderCompletionData
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public DateTime RequestedAt { get; set; }
    }
}
