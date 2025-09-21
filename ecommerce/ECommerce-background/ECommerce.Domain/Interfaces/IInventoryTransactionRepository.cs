using ECommerce.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerce.Domain.Interfaces
{
    /// <summary>
    /// 库存事务仓储接口
    /// </summary>
    public interface IInventoryTransactionRepository
    {
        /// <summary>
        /// 添加库存事务记录
        /// </summary>
        /// <param name="transaction">库存事务</param>
        /// <returns>库存事务</returns>
        Task<InventoryTransaction> AddAsync(InventoryTransaction transaction);

        /// <summary>
        /// 根据产品ID获取库存事务记录
        /// </summary>
        /// <param name="productId">产品ID</param>
        /// <returns>库存事务记录列表</returns>
        Task<IEnumerable<InventoryTransaction>> GetByProductIdAsync(Guid productId);

        /// <summary>
        /// 根据操作类型获取库存事务记录
        /// </summary>
        /// <param name="operationType">操作类型</param>
        /// <returns>库存事务记录列表</returns>
        Task<IEnumerable<InventoryTransaction>> GetByOperationTypeAsync(InventoryOperationType operationType);

        /// <summary>
        /// 获取指定日期范围内的库存事务记录
        /// </summary>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>库存事务记录列表</returns>
        Task<IEnumerable<InventoryTransaction>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// 获取库存事务记录详情
        /// </summary>
        /// <param name="id">库存事务ID</param>
        /// <returns>库存事务记录</returns>
        Task<InventoryTransaction?> GetByIdAsync(Guid id);
    }
}