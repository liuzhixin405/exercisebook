using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Models;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Repositories
{
    /// <summary>
    /// 库存事务仓储实现
    /// </summary>
    public class InventoryTransactionRepository : IInventoryTransactionRepository
    {
        private readonly ECommerceDbContext _context;

        public InventoryTransactionRepository(ECommerceDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 添加库存事务记录
        /// </summary>
        /// <param name="transaction">库存事务</param>
        /// <returns>库存事务</returns>
        public async Task<InventoryTransaction> AddAsync(InventoryTransaction transaction)
        {
            transaction.Id = Guid.NewGuid();
            transaction.CreatedAt = DateTime.UtcNow;
            
            _context.InventoryTransactions.Add(transaction);
            await _context.SaveChangesAsync();
            
            return transaction;
        }

        /// <summary>
        /// 根据产品ID获取库存事务记录
        /// </summary>
        /// <param name="productId">产品ID</param>
        /// <returns>库存事务记录列表</returns>
        public async Task<IEnumerable<InventoryTransaction>> GetByProductIdAsync(Guid productId)
        {
            return await _context.InventoryTransactions
                .Where(t => t.ProductId == productId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// 根据操作类型获取库存事务记录
        /// </summary>
        /// <param name="operationType">操作类型</param>
        /// <returns>库存事务记录列表</returns>
        public async Task<IEnumerable<InventoryTransaction>> GetByOperationTypeAsync(InventoryOperationType operationType)
        {
            return await _context.InventoryTransactions
                .Where(t => t.OperationType == operationType)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// 获取指定日期范围内的库存事务记录
        /// </summary>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>库存事务记录列表</returns>
        public async Task<IEnumerable<InventoryTransaction>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.InventoryTransactions
                .Where(t => t.CreatedAt >= startDate && t.CreatedAt <= endDate)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// 获取库存事务记录详情
        /// </summary>
        /// <param name="id">库存事务ID</param>
        /// <returns>库存事务记录</returns>
        public async Task<InventoryTransaction?> GetByIdAsync(Guid id)
        {
            return await _context.InventoryTransactions
                .FirstOrDefaultAsync(t => t.Id == id);
        }
    }
}