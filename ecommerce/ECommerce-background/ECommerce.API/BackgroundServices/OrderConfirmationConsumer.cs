using ECommerce.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace ECommerce.API.BackgroundServices
{
    public class OrderConfirmationConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OrderConfirmationConsumer> _logger;
        private readonly IRabbitMqConnectionProvider _rabbitProvider;
        private IModel? _channel;
        private string _consumerTag = string.Empty;
        private volatile bool _isStopping = false;

        private const string ConfirmationQueueName = "order.confirmation.queue";

        public OrderConfirmationConsumer(
            IServiceProvider serviceProvider,
            ILogger<OrderConfirmationConsumer> logger,
            IRabbitMqConnectionProvider rabbitProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _rabbitProvider = rabbitProvider;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("OrderConfirmationConsumer is starting (RabbitMQ consumer).");

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

            // 确保队列存在
            try
            {
                _channel.QueueDeclare(queue: ConfirmationQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to declare confirmation queue");
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
                
                try
                {
                    var confirmationData = JsonSerializer.Deserialize<OrderConfirmationData>(message);
                    if (confirmationData != null)
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
                        var messagePublisher = scope.ServiceProvider.GetRequiredService<IOrderMessagePublisher>();

                        // 处理订单确认逻辑
                        var confirmed = await ProcessOrderConfirmation(confirmationData, orderService);
                        if (confirmed)
                        {
                            _logger.LogInformation("OrderConfirmationConsumer: Confirmed order {OrderId} via message queue", confirmationData.OrderId);
                            
                            // 订单确认后，发送发货消息
                            try
                            {
                                await messagePublisher.PublishShipmentMessageAsync(confirmationData.OrderId, confirmationData.UserId);
                                _logger.LogInformation("OrderConfirmationConsumer: Sent shipment message for confirmed order {OrderId}", confirmationData.OrderId);
                            }
                            catch (Exception publishEx)
                            {
                                _logger.LogError(publishEx, "OrderConfirmationConsumer: Failed to publish shipment message for order {OrderId}", confirmationData.OrderId);
                            }
                        }
                        else
                        {
                            _logger.LogInformation("OrderConfirmationConsumer: Order {OrderId} not confirmed (maybe status changed)", confirmationData.OrderId);
                        }

                        _channel?.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                    else
                    {
                        _logger.LogWarning("OrderConfirmationConsumer: Received invalid confirmation message: {Message}", message);
                        _channel?.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                }
                catch (ObjectDisposedException)
                {
                    // 服务正在关闭，忽略此错误
                    _logger.LogWarning("OrderConfirmationConsumer: Service is being disposed, ignoring confirmation message");
                    return;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing confirmation message: {Message}", message);
                    _channel?.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                }

                await Task.Yield();
            };

            _consumerTag = _channel.BasicConsume(queue: ConfirmationQueueName, autoAck: false, consumer: consumer);

            _logger.LogInformation("OrderConfirmationConsumer: Waiting for confirmation messages...");

            return Task.CompletedTask;
        }

        private async Task<bool> ProcessOrderConfirmation(OrderConfirmationData confirmationData, IOrderService orderService)
        {
            try
            {
                // 确认订单（从Paid状态更新为Confirmed状态）
                var confirmed = await orderService.ConfirmOrderAsync(confirmationData.OrderId);
                
                if (confirmed)
                {
                    _logger.LogInformation("Order {OrderId} confirmed successfully", confirmationData.OrderId);
                }
                else
                {
                    _logger.LogWarning("Order {OrderId} could not be confirmed (maybe already confirmed or status changed)", confirmationData.OrderId);
                }
                
                return confirmed;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process confirmation for order {OrderId}", confirmationData.OrderId);
                return false;
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("OrderConfirmationConsumer is stopping.");
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

    public class OrderConfirmationData
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public DateTime RequestedAt { get; set; }
    }
}
