using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.Core.SeedWork
{
    internal abstract class DomainEvent<TType, TKey> : IDomainEvent<TKey> where TType : IAggregateRoot<TKey>
    {
        public long AggregateVersion { get; init; }

        public TKey AggregateId { get; init; }

        public DateTime Timestamp { get; init; }
        protected DomainEvent(TType aggregateRoot)
        {
            AggregateId = aggregateRoot.Id;
            AggregateVersion = aggregateRoot.Version;
            Timestamp = DateTime.UtcNow;
        }
    }
}
