using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Architecture.Domain.Orders
{
    public class OrderShippingConfirmation
    {
        public OrderShippingConfirmation()
        {
            TrackingId = string.Empty; ;
            ExpectedShipDate = DateTime.MaxValue;
        }

        public string TrackingId { get; set; }
        public DateTime ExpectedShipDate { get; set; }
    }
}
