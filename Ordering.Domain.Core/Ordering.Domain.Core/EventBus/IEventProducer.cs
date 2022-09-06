using Ordering.Domain.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.Core.EventBus
{
    internal interface IEventProducer<in TType,in TKey> where TType:IAggregateRoot<TKey>
    {
        Task DispatchAsync(TType aggregateKey);
    }
}
