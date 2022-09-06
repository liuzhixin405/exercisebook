using Orleans.Concurrency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Grains.Interfaces.Models
{
    [Immutable]
    public class TodoNotification
    {
        public TodoNotification(Guid itemKey, TodoItem item)
        {
            ItemKey = itemKey;
            Item = item;
        }

        public Guid ItemKey { get; }
        public TodoItem Item { get; }
    }
}
