using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merp.Infrastructure
{
    public abstract class Aggregate : IAggregate
    {
        public Guid Id { get; protected set; }

        private IList<DomainEvent> uncommittedEvents = new List<DomainEvent>();

        Guid IAggregate.Id 
        { 
            get
            {
                return Id;
            }
        }

        bool IAggregate.HasPendingChanges 
        { 
            get 
            { 
                return this.uncommittedEvents.Any();  
            } 
        }

        IEnumerable<DomainEvent> IAggregate.GetUncommittedEvents()
        {
            return uncommittedEvents.ToArray();
        }

        void IAggregate.ClearUncommittedEvents()
        {
            uncommittedEvents.Clear();
        }

        protected void RaiseEvent(DomainEvent @event)
        {
            uncommittedEvents.Add(@event);
        }
    }
}
