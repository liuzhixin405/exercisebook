using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.Core.SeedWork
{
    internal abstract class AggregateRoot<TType, TKey> : Entity<TKey>, IAggregateRoot<TKey> where TType : class, IAggregateRoot<TKey>
    {
        protected AggregateRoot(TKey id) : base(id)
        {
        }

        public long Version { get;private set; }

        public IReadOnlyCollection<IDomainEvent<TKey>> Events => _events;

        public void ClearEvents()
        {
            _events.Clear();
        }
        private readonly Queue<IDomainEvent<TKey>> _events = new Queue<IDomainEvent<TKey>>();   
    }
}
