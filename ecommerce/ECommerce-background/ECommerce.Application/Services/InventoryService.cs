using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Events;
using ECommerce.Domain.Models;
using ECommerce.Core.EventBus;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Services
{
    /// <summary>
    /// 库存管理服务实现
    /// </summary>
    public class InventoryService : IInventoryService
    {
        private readonly IProductRepository _productRepository;
        private readonly IEventBus _eventBus;
        private readonly IInventoryTransactionRepository _inventoryTransactionRepository;
        private readonly ILogger<InventoryService> _logger;
        private readonly Dictionary<Guid, int> _lockedStock = new(); // 锁定的库存
        private readonly Dictionary<Guid, int> _reservedStock = new(); // 预留的库存

        public InventoryService(
            IProductRepository productRepository, 
            IEventBus eventBus, 
            IInventoryTransactionRepository inventoryTransactionRepository,
            ILogger<InventoryService> logger)
        {
            _productRepository = productRepository;
            _eventBus = eventBus;
            _inventoryTransactionRepository = inventoryTransactionRepository;
            _logger = logger;
        }

        public async Task<InventoryCheckResult> CheckStockAsync(Guid productId, int quantity)
        {
            _logger.LogInformation("Checking stock for product: {ProductId}, requested quantity: {Quantity}", 
                productId, quantity);

            try
            {
                var product = await _productRepository.GetByIdAsync(productId);
                if (product == null)
                {
                    return new InventoryCheckResult
                    {
                        IsAvailable = false,
                        ProductId = productId,
                        RequestedQuantity = quantity,
                        AvailableStock = 0,
                        ReservedStock = 0,
                        Message = "Product not found"
                    };
                }

                var lockedStock = _lockedStock.GetValueOrDefault(productId, 0);
                var reservedStock = _reservedStock.GetValueOrDefault(productId, 0);
                var availableStock = product.Stock - lockedStock - reservedStock;

                var isAvailable = availableStock >= quantity;

                var result = new InventoryCheckResult
                {
                    IsAvailable = isAvailable,
                    ProductId = productId,
                    RequestedQuantity = quantity,
                    AvailableStock = availableStock,
                    ReservedStock = reservedStock,
                    Message = isAvailable ? "Stock is available" : "Insufficient stock"
                };

                _logger.LogInformation("Stock check completed for product: {ProductId}, Available: {Available}, Requested: {Requested}, IsAvailable: {IsAvailable}", 
                    productId, availableStock, quantity, isAvailable);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Stock check failed for product: {ProductId}", productId);

                return new InventoryCheckResult
                {
                    IsAvailable = false,
                    ProductId = productId,
                    RequestedQuantity = quantity,
                    AvailableStock = 0,
                    ReservedStock = 0,
                    Message = $"Stock check failed: {ex.Message}"
                };
            }
        }

        public async Task<InventoryOperationResult> DeductStockAsync(Guid productId, int quantity)
        {
            _logger.LogInformation("Deducting stock for product: {ProductId}, quantity: {Quantity}", 
                productId, quantity);

            try
            {
                var product = await _productRepository.GetByIdAsync(productId);
                if (product == null)
                {
                    return new InventoryOperationResult
                    {
                        Success = false,
                        ProductId = productId,
                        Quantity = quantity,
                        NewStock = 0,
                        Message = "Product not found",
                        OperationTime = DateTime.UtcNow
                    };
                }

                // 检查库存是否充足
                var availableStock = product.Stock - _lockedStock.GetValueOrDefault(productId, 0) - _reservedStock.GetValueOrDefault(productId, 0);
                if (availableStock < quantity)
                {
                    return new InventoryOperationResult
                    {
                        Success = false,
                        ProductId = productId,
                        Quantity = quantity,
                        NewStock = product.Stock,
                        Message = $"Insufficient stock. Available: {availableStock}, Requested: {quantity}",
                        OperationTime = DateTime.UtcNow
                    };
                }

                // 扣减库存
                var oldStock = product.Stock;
                product.Stock -= quantity;
                product.UpdatedAt = DateTime.UtcNow;

                await _productRepository.UpdateAsync(product);

                // 记录库存事务
                var transaction = new InventoryTransaction
                {
                    ProductId = productId,
                    OperationType = InventoryOperationType.Deduct,
                    Quantity = quantity,
                    BeforeStock = oldStock,
                    AfterStock = product.Stock,
                    Reason = "Stock deduction",
                    CreatedBy = "System"
                };
                
                await _inventoryTransactionRepository.AddAsync(transaction);

                var result = new InventoryOperationResult
                {
                    Success = true,
                    ProductId = productId,
                    Quantity = quantity,
                    OldStock = oldStock,
                    NewStock = product.Stock,
                    Message = "Stock deducted successfully",
                    OperationTime = DateTime.UtcNow
                };

                _logger.LogInformation("Stock deducted successfully for product: {ProductId}, Old: {Old}, New: {New}, Deducted: {Deducted}", 
                    productId, oldStock, product.Stock, quantity);

                // 发布库存更新事件
                try
                {
                    var inventoryUpdatedEvent = new InventoryUpdatedEvent(
                        productId,
                        product.Name,
                        oldStock,
                        product.Stock,
                        InventoryOperationType.Deduct,
                        "Stock deduction",
                        Guid.Empty);

                    await _eventBus.PublishAsync(inventoryUpdatedEvent);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to publish inventory updated event for product {ProductId}", productId);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Stock deduction failed for product: {ProductId}", productId);

                return new InventoryOperationResult
                {
                    Success = false,
                    ProductId = productId,
                    Quantity = quantity,
                    NewStock = 0,
                    Message = $"Stock deduction failed: {ex.Message}",
                    OperationTime = DateTime.UtcNow
                };
            }
        }

        public async Task<InventoryOperationResult> RestoreStockAsync(Guid productId, int quantity)
        {
            _logger.LogInformation("Restoring stock for product: {ProductId}, quantity: {Quantity}", 
                productId, quantity);

            try
            {
                var product = await _productRepository.GetByIdAsync(productId);
                if (product == null)
                {
                    return new InventoryOperationResult
                    {
                        Success = false,
                        ProductId = productId,
                        Quantity = quantity,
                        NewStock = 0,
                        Message = "Product not found",
                        OperationTime = DateTime.UtcNow
                    };
                }

                // 恢复库存
                var oldStock = product.Stock;
                product.Stock += quantity;
                product.UpdatedAt = DateTime.UtcNow;

                await _productRepository.UpdateAsync(product);

                // 记录库存事务
                var transaction = new InventoryTransaction
                {
                    ProductId = productId,
                    OperationType = InventoryOperationType.Add,
                    Quantity = quantity,
                    BeforeStock = oldStock,
                    AfterStock = product.Stock,
                    Reason = "Stock restoration",
                    CreatedBy = "System"
                };
                
                await _inventoryTransactionRepository.AddAsync(transaction);

                var result = new InventoryOperationResult
                {
                    Success = true,
                    ProductId = productId,
                    Quantity = quantity,
                    OldStock = oldStock,
                    NewStock = product.Stock,
                    Message = "Stock restored successfully",
                    OperationTime = DateTime.UtcNow
                };

                _logger.LogInformation("Stock restored successfully for product: {ProductId}, Old: {Old}, New: {New}, Restored: {Restored}", 
                    productId, oldStock, product.Stock, quantity);

                // 发布库存更新事件（Add）
                try
                {
                    var inventoryUpdatedEvent = new InventoryUpdatedEvent(
                        productId,
                        product.Name,
                        oldStock,
                        product.Stock,
                        InventoryOperationType.Add,
                        "Stock restoration",
                        Guid.Empty);

                    await _eventBus.PublishAsync(inventoryUpdatedEvent);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to publish inventory updated event (Add) for product {ProductId}", productId);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Stock restoration failed for product: {ProductId}", productId);

                return new InventoryOperationResult
                {
                    Success = false,
                    ProductId = productId,
                    Quantity = quantity,
                    NewStock = 0,
                    Message = $"Stock restoration failed: {ex.Message}",
                    OperationTime = DateTime.UtcNow
                };
            }
        }

        public async Task<InventoryOperationResult> LockStockAsync(Guid productId, int quantity, Guid orderId)
        {
            _logger.LogInformation("Locking stock for product: {ProductId}, quantity: {Quantity}, order: {OrderId}", 
                productId, quantity, orderId);

            try
            {
                var product = await _productRepository.GetByIdAsync(productId);
                if (product == null)
                {
                    return new InventoryOperationResult
                    {
                        Success = false,
                        ProductId = productId,
                        Quantity = quantity,
                        NewStock = 0,
                        Message = "Product not found",
                        OperationTime = DateTime.UtcNow
                    };
                }

                // 检查库存是否充足
                var availableStock = product.Stock - _lockedStock.GetValueOrDefault(productId, 0) - _reservedStock.GetValueOrDefault(productId, 0);
                if (availableStock < quantity)
                {
                    return new InventoryOperationResult
                    {
                        Success = false,
                        ProductId = productId,
                        Quantity = quantity,
                        NewStock = product.Stock,
                        Message = $"Insufficient stock for locking. Available: {availableStock}, Requested: {quantity}",
                        OperationTime = DateTime.UtcNow
                    };
                }

                // 锁定库存
                var currentLocked = _lockedStock.GetValueOrDefault(productId, 0);
                _lockedStock[productId] = currentLocked + quantity;

                // 记录库存事务
                var transaction = new InventoryTransaction
                {
                    ProductId = productId,
                    OperationType = InventoryOperationType.Lock,
                    Quantity = quantity,
                    BeforeStock = product.Stock,
                    AfterStock = product.Stock, // 锁定不影响实际库存
                    OrderId = orderId,
                    Reason = "Stock locking for order",
                    CreatedBy = "System"
                };
                
                await _inventoryTransactionRepository.AddAsync(transaction);

                var result = new InventoryOperationResult
                {
                    Success = true,
                    ProductId = productId,
                    Quantity = quantity,
                    NewStock = product.Stock,
                    Message = "Stock locked successfully",
                    OperationTime = DateTime.UtcNow
                };

                _logger.LogInformation("Stock locked successfully for product: {ProductId}, Order: {OrderId}, Locked: {Locked}", 
                    productId, orderId, _lockedStock[productId]);

                // 发布库存更新事件（Lock）
                try
                {
                    var inventoryUpdatedEvent = new InventoryUpdatedEvent(
                        productId,
                        product.Name,
                        product.Stock, // 锁定前后实际库存不变
                        product.Stock,
                        InventoryOperationType.Lock,
                        "Stock locking for order",
                        orderId);

                    await _eventBus.PublishAsync(inventoryUpdatedEvent);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to publish inventory updated event (Lock) for product {ProductId}", productId);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Stock locking failed for product: {ProductId}", productId);

                return new InventoryOperationResult
                {
                    Success = false,
                    ProductId = productId,
                    Quantity = quantity,
                    NewStock = 0,
                    Message = $"Stock locking failed: {ex.Message}",
                    OperationTime = DateTime.UtcNow
                };
            }
        }

        public async Task<InventoryOperationResult> ReleaseLockedStockAsync(Guid productId, int quantity, Guid orderId)
        {
            _logger.LogInformation("Releasing locked stock for product: {ProductId}, quantity: {Quantity}, order: {OrderId}", 
                productId, quantity, orderId);

            try
            {
                var currentLocked = _lockedStock.GetValueOrDefault(productId, 0);
                if (currentLocked < quantity)
                {
                    return new InventoryOperationResult
                    {
                        Success = false,
                        ProductId = productId,
                        Quantity = quantity,
                        NewStock = 0,
                        Message = $"Cannot release more stock than locked. Locked: {currentLocked}, Requested: {quantity}",
                        OperationTime = DateTime.UtcNow
                    };
                }

                // 释放锁定的库存
                _lockedStock[productId] = currentLocked - quantity;

                // 记录库存事务
                var transaction = new InventoryTransaction
                {
                    ProductId = productId,
                    OperationType = InventoryOperationType.Unlock,
                    Quantity = quantity,
                    BeforeStock = 0, // 释放不影响实际库存
                    AfterStock = 0,  // 释放不影响实际库存
                    OrderId = orderId,
                    Reason = "Stock unlocking for order",
                    CreatedBy = "System"
                };
                
                await _inventoryTransactionRepository.AddAsync(transaction);

                var result = new InventoryOperationResult
                {
                    Success = true,
                    ProductId = productId,
                    Quantity = quantity,
                    NewStock = 0,
                    Message = "Locked stock released successfully",
                    OperationTime = DateTime.UtcNow
                };

                _logger.LogInformation("Locked stock released successfully for product: {ProductId}, Order: {OrderId}, Remaining locked: {Remaining}", 
                    productId, orderId, _lockedStock[productId]);

                // 发布库存更新事件（Unlock）
                try
                {
                    var inventoryUpdatedEvent = new InventoryUpdatedEvent(
                        productId,
                        string.Empty,
                        0,
                        0,
                        InventoryOperationType.Unlock,
                        "Stock unlocking for order",
                        orderId);

                    await _eventBus.PublishAsync(inventoryUpdatedEvent);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to publish inventory updated event (Unlock) for product {ProductId}", productId);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Stock release failed for product: {ProductId}", productId);

                return new InventoryOperationResult
                {
                    Success = false,
                    ProductId = productId,
                    Quantity = quantity,
                    NewStock = 0,
                    Message = $"Stock release failed: {ex.Message}",
                    OperationTime = DateTime.UtcNow
                };
            }
        }

        public async Task<ProductInventoryInfo> GetProductInventoryAsync(Guid productId)
        {
            _logger.LogInformation("Getting inventory info for product: {ProductId}", productId);

            try
            {
                var product = await _productRepository.GetByIdAsync(productId);
                if (product == null)
                {
                    return new ProductInventoryInfo
                    {
                        ProductId = productId,
                        ProductName = string.Empty,
                        TotalStock = 0,
                        AvailableStock = 0,
                        ReservedStock = 0,
                        LockedStock = 0,
                        LastUpdated = DateTime.UtcNow,
                        IsLowStock = false,
                        LowStockThreshold = 0
                    };
                }

                var lockedStock = _lockedStock.GetValueOrDefault(productId, 0);
                var reservedStock = _reservedStock.GetValueOrDefault(productId, 0);
                var availableStock = product.Stock - lockedStock - reservedStock;
                var lowStockThreshold = Math.Max(1, product.Stock / 10); // 低库存阈值为总库存的10%

                var result = new ProductInventoryInfo
                {
                    ProductId = productId,
                    ProductName = product.Name,
                    TotalStock = product.Stock,
                    AvailableStock = availableStock,
                    ReservedStock = reservedStock,
                    LockedStock = lockedStock,
                    LastUpdated = product.UpdatedAt,
                    IsLowStock = availableStock <= lowStockThreshold,
                    LowStockThreshold = lowStockThreshold
                };

                _logger.LogInformation("Inventory info retrieved for product: {ProductId}, Total: {Total}, Available: {Available}, Locked: {Locked}, Reserved: {Reserved}", 
                    productId, result.TotalStock, result.AvailableStock, result.LockedStock, result.ReservedStock);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get inventory info for product: {ProductId}", productId);

                return new ProductInventoryInfo
                {
                    ProductId = productId,
                    ProductName = string.Empty,
                    TotalStock = 0,
                    AvailableStock = 0,
                    ReservedStock = 0,
                    LockedStock = 0,
                    LastUpdated = DateTime.UtcNow,
                    IsLowStock = false,
                    LowStockThreshold = 0
                };
            }
        }

        /// <summary>
        /// 获取产品的库存事务记录
        /// </summary>
        /// <param name="productId">产品ID</param>
        /// <param name="limit">返回记录数量限制</param>
        /// <returns>库存事务记录列表</returns>
        public async Task<IEnumerable<InventoryTransaction>> GetProductInventoryTransactionsAsync(Guid productId, int limit = 50)
        {
            _logger.LogInformation("Getting inventory transactions for product: {ProductId}, limit: {Limit}", productId, limit);

            try
            {
                var transactions = await _inventoryTransactionRepository.GetByProductIdAsync(productId);
                return transactions.Take(limit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get inventory transactions for product: {ProductId}", productId);
                return Enumerable.Empty<InventoryTransaction>();
            }
        }

        /// <summary>
        /// 获取库存事务记录详情
        /// </summary>
        /// <param name="transactionId">事务ID</param>
        /// <returns>库存事务记录</returns>
        public async Task<InventoryTransaction?> GetInventoryTransactionAsync(Guid transactionId)
        {
            _logger.LogInformation("Getting inventory transaction: {TransactionId}", transactionId);

            try
            {
                return await _inventoryTransactionRepository.GetByIdAsync(transactionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get inventory transaction: {TransactionId}", transactionId);
                return null;
            }
        }

        public async Task<BatchInventoryUpdateResult> BatchUpdateInventoryAsync(IEnumerable<InventoryUpdate> updates)
        {
            _logger.LogInformation("Processing batch inventory update for {Count} items", updates.Count());

            var results = new List<InventoryOperationResult>();
            var totalOperations = 0;
            var successfulOperations = 0;
            var failedOperations = 0;

            try
            {
                foreach (var update in updates)
                {
                    totalOperations++;
                    InventoryOperationResult result;

                    switch (update.OperationType)
                    {
                        case InventoryOperationType.Add:
                            result = await RestoreStockAsync(update.ProductId, update.Quantity);
                            break;
                        case InventoryOperationType.Deduct:
                            result = await DeductStockAsync(update.ProductId, update.Quantity);
                            break;
                        case InventoryOperationType.Lock:
                            result = await LockStockAsync(update.ProductId, update.Quantity, update.OrderId ?? Guid.Empty);
                            break;
                        case InventoryOperationType.Unlock:
                            result = await ReleaseLockedStockAsync(update.ProductId, update.Quantity, update.OrderId ?? Guid.Empty);
                            break;
                        default:
                            result = new InventoryOperationResult
                            {
                                Success = false,
                                ProductId = update.ProductId,
                                Quantity = update.Quantity,
                                NewStock = 0,
                                Message = $"Unsupported operation type: {update.OperationType}",
                                OperationTime = DateTime.UtcNow
                            };
                            break;
                    }

                    results.Add(result);
                    if (result.Success)
                        successfulOperations++;
                    else
                        failedOperations++;
                }

                var batchResult = new BatchInventoryUpdateResult
                {
                    OverallSuccess = failedOperations == 0,
                    TotalOperations = totalOperations,
                    SuccessfulOperations = successfulOperations,
                    FailedOperations = failedOperations,
                    Results = results,
                    Message = $"Batch update completed. Success: {successfulOperations}, Failed: {failedOperations}"
                };

                _logger.LogInformation("Batch inventory update completed. Total: {Total}, Success: {Success}, Failed: {Failed}", 
                    totalOperations, successfulOperations, failedOperations);

                return batchResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Batch inventory update failed");

                return new BatchInventoryUpdateResult
                {
                    OverallSuccess = false,
                    TotalOperations = totalOperations,
                    SuccessfulOperations = successfulOperations,
                    FailedOperations = failedOperations,
                    Results = results,
                    Message = $"Batch update failed: {ex.Message}"
                };
            }
        }
    }
}
