namespace ECommerce.Domain.Models
{
    /// <summary>
    /// 库存检查结果
    /// </summary>
    public class InventoryCheckResult
    {
        public bool IsAvailable { get; set; }
        public Guid ProductId { get; set; }
        public int RequestedQuantity { get; set; }
        public int AvailableStock { get; set; }
        public int ReservedStock { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// 库存操作结果
    /// </summary>
    public class InventoryOperationResult
    {
        public bool Success { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public int OldStock { get; set; }
        public int NewStock { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime OperationTime { get; set; }
    }

    /// <summary>
    /// 产品库存信息
    /// </summary>
    public class ProductInventoryInfo
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int TotalStock { get; set; }
        public int AvailableStock { get; set; }
        public int ReservedStock { get; set; }
        public int LockedStock { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool IsLowStock { get; set; }
        public int LowStockThreshold { get; set; }
    }

    /// <summary>
    /// 库存更新
    /// </summary>
    public class InventoryUpdate
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public InventoryOperationType OperationType { get; set; }
        public string Reason { get; set; } = string.Empty;
        public Guid? OrderId { get; set; }
        public string Notes { get; set; } = string.Empty;
    }

    /// <summary>
    /// 批量库存更新结果
    /// </summary>
    public class BatchInventoryUpdateResult
    {
        public bool OverallSuccess { get; set; }
        public int TotalOperations { get; set; }
        public int SuccessfulOperations { get; set; }
        public int FailedOperations { get; set; }
        public List<InventoryOperationResult> Results { get; set; } = new();
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// 库存操作类型
    /// </summary>
    public enum InventoryOperationType
    {
        Add,            // 增加库存
        Deduct,         // 扣减库存
        Reserve,        // 预留库存
        Release,        // 释放预留
        Lock,           // 锁定库存
        Unlock,         // 解锁库存
        Adjust,         // 调整库存
        Set             // 设置库存
    }

    /// <summary>
    /// 库存事务记录
    /// </summary>
    public class InventoryTransaction
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public InventoryOperationType OperationType { get; set; }
        public int Quantity { get; set; }
        public int BeforeStock { get; set; }
        public int AfterStock { get; set; }
        public Guid? OrderId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }
}
