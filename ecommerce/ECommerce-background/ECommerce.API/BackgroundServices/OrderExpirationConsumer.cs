using ECommerce.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace ECommerce.API.BackgroundServices
{
    public class OrderExpirationConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OrderExpirationConsumer> _logger;
        private readonly IRabbitMqConnectionProvider _rabbitProvider;
        private IModel? _channel;
        private string _consumerTag = string.Empty;
        private volatile bool _isStopping = false;

        private const string ExpiredQueueName = "order.expired.queue";

        public OrderExpirationConsumer(
            IServiceProvider serviceProvider,
            ILogger<OrderExpirationConsumer> logger,
            IRabbitMqConnectionProvider rabbitProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _rabbitProvider = rabbitProvider;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("OrderExpirationConsumer is starting (RabbitMQ consumer).");

            _rabbitProvider.EnsureQueuesAndExchanges();
            _channel = _rabbitProvider.GetChannel();

            return base.StartAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_channel == null)
            {
                _logger.LogError("RabbitMQ channel is not initialized.");
                return Task.CompletedTask;
            }

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                // 检查是否正在停止
                if (_isStopping)
                {
                    _channel?.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    return;
                }

                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                if (Guid.TryParse(message, out Guid orderId))
                {
                    try
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();

                        var cancelled = await orderService.CancelOrderAsync(orderId);
                        if (cancelled)
                        {
                            _logger.LogInformation("OrderExpirationConsumer: Cancelled order {OrderId} via delayed message", orderId);
                        }
                        else
                        {
                            _logger.LogInformation("OrderExpirationConsumer: Order {OrderId} not cancelled (maybe already paid or changed)", orderId);
                        }

                        _channel?.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                    catch (ObjectDisposedException)
                    {
                        // 服务正在关闭，忽略此错误
                        _logger.LogWarning("OrderExpirationConsumer: Service is being disposed, ignoring message for order {OrderId}", orderId);
                        return;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing expired order message for order {OrderId}", orderId);
                        _channel?.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                    }
                }
                else
                {
                    _logger.LogWarning("OrderExpirationConsumer: Received invalid expired-order message: {Message}", message);
                    _channel?.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }

                await Task.Yield();
            };

            _consumerTag = _channel.BasicConsume(queue: ExpiredQueueName, autoAck: false, consumer: consumer);

            _logger.LogInformation("OrderExpirationConsumer: Waiting for expired order messages...");

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("OrderExpirationConsumer is stopping.");
            _isStopping = true;

            try
            {
                if (!string.IsNullOrEmpty(_consumerTag) && _channel != null)
                {
                    _channel.BasicCancel(_consumerTag);
                }

                _channel?.Close();
                _rabbitProvider.GetConnection()?.Close();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while stopping RabbitMQ consumer");
            }

            return base.StopAsync(cancellationToken);
        }
    }
}