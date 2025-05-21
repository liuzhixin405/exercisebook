using System;
using System.Collections.Generic;

namespace Pandora.Cigfi.Models.Cigfi.Order
{
    public class Order
    {
        public long Id { get; set; }
        public string OrderNo { get; set; }
        public long UserId { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PayAmount { get; set; }
        public string PayStatus { get; set; }
        public DateTime? PayTime { get; set; }
        public long? AddressId { get; set; }
        public string Remark { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        // 订单明细表单独维护
    }

    public enum OrderStatus
    {
        PendingPayment,
        PendingShipment,
        PendingReceipt,
        Completed,
        Cancelled
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
