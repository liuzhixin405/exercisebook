using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.Core
{
    internal interface IEventService<TType,TKey>
    {
        Task PersistAsync(TType aggregateRoot);
    }
}
