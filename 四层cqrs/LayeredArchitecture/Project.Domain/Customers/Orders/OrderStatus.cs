using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.Customers.Orders
{
    public enum OrderStatus
    {
        Placed = 0,
        InRealization = 1,
        Canceled = 2,
        Delivered = 3,
        Sent = 4,
        WaitingForPayment = 5
    }
}
