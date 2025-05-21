using System;
using System.Collections.Generic;

namespace Pandora.Cigfi.Models.Cigfi.Order
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public string OrderNo { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PayAmount { get; set; }
        public bool PayStatus { get; set; }
        public string Remark { get; set; }
        public string CreatedAt { get; set; }
        public List<OrderItemViewModel> Items { get; set; }
    }

    public class OrderItemViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
