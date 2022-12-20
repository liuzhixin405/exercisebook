using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Architecture.Domain.Orders
{
    public enum OrderState
    {
        Canceled = 0,
        Pending = 1,
        Shipped = 2,
        Archived = 3
    }
}
