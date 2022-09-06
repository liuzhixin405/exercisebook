using Contract.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.Core.Events
{
    public record PendingOrderEvent
    {
        public PendingOrderEvent(Order order)
        {

        }
    }
}
