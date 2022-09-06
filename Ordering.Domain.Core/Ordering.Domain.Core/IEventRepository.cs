using Ordering.Domain.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.Core
{
    internal interface IEventRepository<TType,TKey> where TType:class,IAggregateRoot<TKey>
    {
        Task AppendAsync(TType aggregateRoot);
        Task<TType> RehydrateAsync(TKey key);
    }
}
