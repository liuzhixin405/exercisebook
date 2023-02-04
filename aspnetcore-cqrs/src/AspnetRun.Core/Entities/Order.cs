using AspnetRun.Core.Entities.Base;
using System.Collections.Generic;

namespace AspnetRun.Core.Entities
{
    public class Order : Entity
    {
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public int BillingAddressId { get; set; }
        public Address BillingAddress { get; set; }
        public int ShippingAddressId { get; set; }
        public Address ShippingAddress { get; set; }
        public OrderStatus Status { get; set; }
        public decimal GrandTotal { get; set; }

        public List<OrderItem> Items { get; set; } = new List<OrderItem>();

        // n-n relationships
        public IList<OrderPaymentAssociation> Payments { get; set; } = new List<OrderPaymentAssociation>();
    }

    public enum OrderStatus
    {
        Draft = 1,
        Canceled = 2,
        Closed = 3
    }
}
