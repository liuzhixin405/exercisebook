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
    public class OrderCompletionConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OrderCompletionConsumer> _logger;
        private readonly IRabbitMqConnectionProvider _rabbitProvider;
        private IModel? _channel;
        private string _consumerTag = string.Empty;
        private volatile bool _isStopping = false;

        private const string CompletionQueueName = "order.completion.queue";

        public OrderCompletionConsumer(
            IServiceProvider serviceProvider,
            ILogger<OrderCompletionConsumer> logger,
            IRabbitMqConnectionProvider rabbitProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _rabbitProvider = rabbitProvider;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("OrderCompletionConsumer is starting (RabbitMQ consumer).");

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
                
                try
                {
                    var completionData = JsonSerializer.Deserialize<OrderCompletionData>(message);
                    if (completionData != null)
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();

                        // 处理订单完成逻辑
                        var completed = await ProcessOrderCompletion(completionData, orderService);
                        if (completed)
                        {
                            _logger.LogInformation("OrderCompletionConsumer: Completed order {OrderId} via message queue", completionData.OrderId);
                        }
                        else
                        {
                            _logger.LogInformation("OrderCompletionConsumer: Order {OrderId} not completed (maybe status changed)", completionData.OrderId);
                        }

                        _channel?.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                    else
                    {
                        _logger.LogWarning("OrderCompletionConsumer: Received invalid completion message: {Message}", message);
                        _channel?.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                }
                catch (ObjectDisposedException)
                {
                    // 服务正在关闭，忽略此错误
                    _logger.LogWarning("OrderCompletionConsumer: Service is being disposed, ignoring completion message");
                    return;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing completion message: {Message}", message);
                    _channel?.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                }

                await Task.Yield();
            };

            _consumerTag = _channel.BasicConsume(queue: CompletionQueueName, autoAck: false, consumer: consumer);

            _logger.LogInformation("OrderCompletionConsumer: Waiting for completion messages...");

            return Task.CompletedTask;
        }

        private async Task<bool> ProcessOrderCompletion(OrderCompletionData completionData, IOrderService orderService)
        {
            try
            {
                // 完成订单（确认收货）
                var completed = await orderService.CompleteOrderAsync(completionData.OrderId);
                
                if (completed)
                {
                    _logger.LogInformation("Order {OrderId} completed successfully", completionData.OrderId);
                }
                
                return completed;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process completion for order {OrderId}", completionData.OrderId);
                return false;
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("OrderCompletionConsumer is stopping.");
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

    public class OrderCompletionData
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public DateTime RequestedAt { get; set; }
    }
}
