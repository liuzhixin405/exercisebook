using CqrsLibrary.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CqrsLibrary.Domain
{
    public abstract class AggregationRoot
    {
        private readonly List<Event> changes = new List<Event>();

        public abstract Guid Id { get; }
        public int Version { get;internal set; }

        public IEnumerable<Event> GetUnCommittedChanged()
        {
            return changes;
        }

        public void MarkChangesAsCommitted()
        {
            changes.Clear();
        }

        public void LoadsFormHistory(IEnumerable<Event> history)
        {
            foreach (Event e in history) ApplyChange(e, false);
        }
        protected void ApplyChange(Event e)
        {
            ApplyChange(e, true);
        }
        private void ApplyChange(Event e, bool v)
        {
            this.AsDynamic().Apply(@e);
            if (v) changes.Add(e);
        }
    }

    public interface IRepository<T> where T : AggregationRoot, new()
    {
        void Save(AggregationRoot aggregate,int expectedVersion);
        T GetById(Guid id);
    }

    public class Repository<T> : IRepository<T> where T : AggregationRoot, new()
    {
        private readonly IEventStore store;
        public Repository(IEventStore store)
        {
            this.store = store;
        }

        public T GetById(Guid id)
        {
            var obj = new T();
            var e = store.GetEventsForAggregate(id);
            obj.LoadsFormHistory(e);
            return obj;
        }

        public void Save(AggregationRoot aggregate, int expectedVersion)
        {
            store.SaveEvents(aggregate.Id, aggregate.GetUnCommittedChanged(), expectedVersion);
        }
    }
}
