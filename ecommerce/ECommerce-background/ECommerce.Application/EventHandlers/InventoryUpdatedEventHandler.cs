using ECommerce.Core.EventBus;
using ECommerce.Domain.Events;
using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Models;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.EventHandlers
{
    /// <summary>
    /// 库存更新事件处理器 - 简化版本，只做核心业务逻辑确认
    /// </summary>
    public class InventoryUpdatedEventHandler : IEventHandler<InventoryUpdatedEvent>
    {
        private readonly ILogger<InventoryUpdatedEventHandler> _logger;
        private readonly IProductRepository _productRepository;
        private readonly ICacheService _cacheService;
        private readonly INotificationService _notificationService;
        private readonly IStatisticsService _statisticsService;

        public InventoryUpdatedEventHandler(
            ILogger<InventoryUpdatedEventHandler> logger,
            IProductRepository productRepository,
            ICacheService cacheService,
            INotificationService notificationService,
            IStatisticsService statisticsService)
        {
            _logger = logger;
            _productRepository = productRepository;
            _cacheService = cacheService;
            _notificationService = notificationService;
            _statisticsService = statisticsService;
        }

        public async Task<bool> HandleAsync(InventoryUpdatedEvent domainEvent, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("InventoryUpdatedEventHandler: Processing inventory update for product {ProductId}", domainEvent.ProductId);

                // 核心业务逻辑：库存更新确认
                await ProcessInventoryUpdateAsync(domainEvent, cancellationToken);

                _logger.LogInformation("InventoryUpdatedEventHandler: Successfully processed inventory update for product {ProductId}", domainEvent.ProductId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "InventoryUpdatedEventHandler: Error processing inventory update for product {ProductId}", domainEvent.ProductId);
                return false;
            }
        }

        /// <summary>
        /// 处理库存更新的核心业务逻辑
        /// </summary>
        private async Task ProcessInventoryUpdateAsync(InventoryUpdatedEvent domainEvent, CancellationToken cancellationToken)
        {
            // 1) 同步产品最新库存到读模型（Product.Stock 已由服务侧持久化，这里确保读侧一致并更新时间）
            var product = await _productRepository.GetByIdAsync(domainEvent.ProductId);
            if (product != null)
            {
                var originalUpdatedAt = product.UpdatedAt;
                product.Stock = domainEvent.NewStock;
                product.UpdatedAt = DateTime.UtcNow;
                await _productRepository.UpdateAsync(product);
                _logger.LogInformation("Synchronized product stock for {ProductId}: {Old} -> {New}", domainEvent.ProductId, domainEvent.OldStock, domainEvent.NewStock);
            }

            // 2) 更新统计
            await _statisticsService.UpdateInventoryStatisticsAsync(domainEvent);

            // 3) 低库存与操作类型通知
            if (domainEvent.NewStock <= 10)
            {
                await _notificationService.SendLowStockNotificationAsync(domainEvent);
            }

            switch (domainEvent.OperationType)
            {
                case InventoryOperationType.Deduct:
                    await _notificationService.SendInventoryOutNotificationAsync(domainEvent);
                    break;
                case InventoryOperationType.Add:
                    await _notificationService.SendInventoryInNotificationAsync(domainEvent);
                    break;
                case InventoryOperationType.Lock:
                    await _notificationService.SendInventoryLockNotificationAsync(domainEvent);
                    break;
                case InventoryOperationType.Unlock:
                    // 可按需发送解锁通知
                    break;
                default:
                    await _notificationService.SendInventoryChangeNotificationAsync(domainEvent);
                    break;
            }

            // 4) 失效相关缓存
            var cacheKeyPrefix = $"product:{domainEvent.ProductId}";
            await _cacheService.RemoveByPatternAsync(cacheKeyPrefix);

            _logger.LogInformation("Inventory update handled for product {ProductId}", domainEvent.ProductId);
        }
    }
}
