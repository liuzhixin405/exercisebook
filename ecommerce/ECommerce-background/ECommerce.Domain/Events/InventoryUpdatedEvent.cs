using ECommerce.Domain.Entities;
using ECommerce.Domain.Models;

namespace ECommerce.Domain.Events
{
    public class InventoryUpdatedEvent : BaseEvent
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int OldStock { get; set; }
        public int NewStock { get; set; }
        public int ChangeAmount { get; set; } // 正数表示增加，负数表示减少
        public InventoryOperationType OperationType { get; set; }
        public string Reason { get; set; } = string.Empty; // 库存变更原因
        public Guid? RelatedOrderId { get; set; } // 关联的订单ID
        public string? Operator { get; set; } // 操作人
        public Dictionary<string, object> OperationData { get; set; } = new(); // 操作相关数据

        public InventoryUpdatedEvent(Guid productId, string productName, int oldStock, int newStock, InventoryOperationType operationType, string reason, Guid? relatedOrderId = null, string? operatorName = null)
        {
            ProductId = productId;
            ProductName = productName;
            OldStock = oldStock;
            NewStock = newStock;
            ChangeAmount = newStock - oldStock;
            OperationType = operationType;
            Reason = reason;
            RelatedOrderId = relatedOrderId;
            Operator = operatorName;
            CorrelationId = relatedOrderId?.ToString() ?? productId.ToString();
            Source = "InventoryService";
            
            // 设置操作相关数据
            OperationData["OperationType"] = operationType.ToString();
            OperationData["ChangeAmount"] = ChangeAmount;
            if (relatedOrderId.HasValue)
            {
                OperationData["OrderId"] = relatedOrderId.Value;
            }
        }
    }
}
