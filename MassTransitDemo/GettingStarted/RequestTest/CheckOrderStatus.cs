using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GettingStarted.RequestTest
{
    internal interface CheckOrderStatus
    {
        String OrderId { get; }
    }

    public interface OrderStatusResult
    {
        public string OrderId { get; }
        public DateTime Timestamp { get; }
        public short StatusCode { get; }
        public string StatusText { get; }
    }

}
