using ECommerce.Domain.Models;
using System.Collections.Generic;

namespace ECommerce.Domain.Interfaces
{
    public interface IInventoryService
    {
        /// <summary>
        /// 检查库存是否充足
        /// </summary>
        /// <param name="productId">产品ID</param>
        /// <param name="quantity">数量</param>
        /// <returns>库存检查结果</returns>
        Task<InventoryCheckResult> CheckStockAsync(Guid productId, int quantity);
        
        /// <summary>
        /// 扣减库存（下单时）
        /// </summary>
        /// <param name="productId">产品ID</param>
        /// <param name="quantity">数量</param>
        /// <returns>扣减结果</returns>
        Task<InventoryOperationResult> DeductStockAsync(Guid productId, int quantity);
        
        /// <summary>
        /// 恢复库存（取消订单时）
        /// </summary>
        /// <param name="productId">产品ID</param>
        /// <param name="quantity">数量</param>
        /// <returns>恢复结果</returns>
        Task<InventoryOperationResult> RestoreStockAsync(Guid productId, int quantity);
        
        /// <summary>
        /// 锁定库存（预占）
        /// </summary>
        /// <param name="productId">产品ID</param>
        /// <param name="quantity">数量</param>
        /// <param name="orderId">订单ID</param>
        /// <returns>锁定结果</returns>
        Task<InventoryOperationResult> LockStockAsync(Guid productId, int quantity, Guid orderId);
        
        /// <summary>
        /// 释放锁定的库存
        /// </summary>
        /// <param name="productId">产品ID</param>
        /// <param name="quantity">数量</param>
        /// <param name="orderId">订单ID</param>
        /// <returns>释放结果</returns>
        Task<InventoryOperationResult> ReleaseLockedStockAsync(Guid productId, int quantity, Guid orderId);
        
        /// <summary>
        /// 获取产品库存信息
        /// </summary>
        /// <param name="productId">产品ID</param>
        /// <returns>库存信息</returns>
        Task<ProductInventoryInfo> GetProductInventoryAsync(Guid productId);
        
        /// <summary>
        /// 获取产品的库存事务记录
        /// </summary>
        /// <param name="productId">产品ID</param>
        /// <param name="limit">返回记录数量限制</param>
        /// <returns>库存事务记录列表</returns>
        Task<IEnumerable<InventoryTransaction>> GetProductInventoryTransactionsAsync(Guid productId, int limit = 50);
        
        /// <summary>
        /// 获取库存事务记录详情
        /// </summary>
        /// <param name="transactionId">事务ID</param>
        /// <returns>库存事务记录</returns>
        Task<InventoryTransaction?> GetInventoryTransactionAsync(Guid transactionId);
        
        /// <summary>
        /// 批量更新库存
        /// </summary>
        /// <param name="updates">库存更新列表</param>
        /// <returns>更新结果</returns>
        Task<BatchInventoryUpdateResult> BatchUpdateInventoryAsync(IEnumerable<InventoryUpdate> updates);
    }
}
