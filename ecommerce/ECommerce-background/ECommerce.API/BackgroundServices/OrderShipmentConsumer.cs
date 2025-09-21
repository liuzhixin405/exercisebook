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
    public class OrderShipmentConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OrderShipmentConsumer> _logger;
        private readonly IRabbitMqConnectionProvider _rabbitProvider;
        private IModel? _channel;
        private string _consumerTag = string.Empty;
        private volatile bool _isStopping = false;

        private const string ShipmentQueueName = "order.shipment.queue";

        public OrderShipmentConsumer(
            IServiceProvider serviceProvider,
            ILogger<OrderShipmentConsumer> logger,
            IRabbitMqConnectionProvider rabbitProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _rabbitProvider = rabbitProvider;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("OrderShipmentConsumer is starting (RabbitMQ consumer).");

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
                    var shipmentData = JsonSerializer.Deserialize<OrderShipmentData>(message);
                    if (shipmentData != null)
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();

                        // 处理发货逻辑
                        var shipped = await ProcessOrderShipment(shipmentData, orderService);
                        if (shipped)
                        {
                            _logger.LogInformation("OrderShipmentConsumer: Shipped order {OrderId} via message queue", shipmentData.OrderId);
                        }
                        else
                        {
                            _logger.LogInformation("OrderShipmentConsumer: Order {OrderId} not shipped (maybe status changed)", shipmentData.OrderId);
                        }

                        _channel?.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                    else
                    {
                        _logger.LogWarning("OrderShipmentConsumer: Received invalid shipment message: {Message}", message);
                        _channel?.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                }
                catch (ObjectDisposedException)
                {
                    // 服务正在关闭，忽略此错误
                    _logger.LogWarning("OrderShipmentConsumer: Service is being disposed, ignoring shipment message");
                    return;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing shipment message: {Message}", message);
                    _channel?.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                }

                await Task.Yield();
            };

            _consumerTag = _channel.BasicConsume(queue: ShipmentQueueName, autoAck: false, consumer: consumer);

            _logger.LogInformation("OrderShipmentConsumer: Waiting for shipment messages...");

            return Task.CompletedTask;
        }

        private async Task<bool> ProcessOrderShipment(OrderShipmentData shipmentData, IOrderService orderService)
        {
            try
            {
                // 生成跟踪号
                var trackingNumber = $"TRK{DateTime.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
                
                // 发货订单
                var shipped = await orderService.ShipOrderAsync(shipmentData.OrderId, trackingNumber);
                
                if (shipped)
                {
                    _logger.LogInformation("Order {OrderId} shipped with tracking number: {TrackingNumber}", 
                        shipmentData.OrderId, trackingNumber);
                }
                
                return shipped;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process shipment for order {OrderId}", shipmentData.OrderId);
                return false;
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("OrderShipmentConsumer is stopping.");
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

    public class OrderShipmentData
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public DateTime RequestedAt { get; set; }
    }
}
