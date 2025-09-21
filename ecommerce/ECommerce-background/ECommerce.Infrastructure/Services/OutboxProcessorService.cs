using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Models;
using ECommerce.Core.EventBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ECommerce.Infrastructure.Services
{
    public class OutboxProcessorService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OutboxProcessorService> _logger;
        private readonly TimeSpan _processingInterval = TimeSpan.FromSeconds(30);
        private readonly TimeSpan _retryInterval = TimeSpan.FromMinutes(5);

        public OutboxProcessorService(
            IServiceProvider serviceProvider,
            ILogger<OutboxProcessorService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Outbox processor service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessPendingMessagesAsync(stoppingToken);
                    await ProcessRetryMessagesAsync(stoppingToken);
                    await CleanupOldMessagesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing outbox messages");
                }

                await Task.Delay(_processingInterval, stoppingToken);
            }

            _logger.LogInformation("Outbox processor service stopped");
        }

        private async Task ProcessPendingMessagesAsync(CancellationToken cancellationToken)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var outboxService = scope.ServiceProvider.GetRequiredService<IOutboxService>();
                var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();

                var pendingMessages = await outboxService.GetPendingEventsAsync(50);
                
                foreach (var message in pendingMessages)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    try
                    {
                        // 使用新的事件总线发布事件
                        // TODO: 实现outbox消息的处理逻辑
                        _logger.LogDebug("Would process outbox message {MessageId} with new event bus", message.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to process outbox message {MessageId}", message.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing pending messages");
            }
        }

        private async Task ProcessRetryMessagesAsync(CancellationToken cancellationToken)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var outboxService = scope.ServiceProvider.GetRequiredService<IOutboxService>();
                var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();

                var retryMessages = await outboxService.GetFailedEventsForRetryAsync(20);
                
                foreach (var message in retryMessages)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    try
                    {
                        // 使用新的事件总线发布事件
                        // TODO: 实现outbox消息的重试逻辑
                        _logger.LogInformation("Would retry outbox message {MessageId} with new event bus", message.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to retry outbox message {MessageId}", message.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing retry messages");
            }
        }

        private async Task CleanupOldMessagesAsync(CancellationToken cancellationToken)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var outboxService = scope.ServiceProvider.GetRequiredService<IOutboxService>();

                // 清理7天前已完成的消息
                var cutoffDate = DateTime.UtcNow.AddDays(-7);
                await outboxService.CleanupCompletedEventsAsync(cutoffDate);
                
                _logger.LogDebug("Cleaned up old completed messages");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while cleaning up old messages");
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping outbox processor service...");
            await base.StopAsync(cancellationToken);
        }
    }
}
