using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merp.Infrastructure.Impl
{
    public class InMemoryRepository : IRepository 
    {
        private Dictionary<Guid, IAggregate> aggregates = new Dictionary<Guid, IAggregate>();
        public IBus Bus { get; private set; }

        public InMemoryRepository(IBus bus)
        {
            if(bus==null)
            {
                throw new ArgumentNullException("bus");
            }
            Bus = bus;
        }


        public void Save<T>(T item) where T : IAggregate
        {
            ManageUncommittedEvents<T>(item);
            PersistAggregate(item);   
        }

        private void PersistAggregate<T>(T item) where T : IAggregate
        {
            if(!aggregates.ContainsKey(item.Id))
            {
                aggregates.Add(item.Id, item);
            }
            else
            {
                aggregates[item.Id] = item;
            }
        }

        private void ManageUncommittedEvents<T>(T item) where T : IAggregate
        {
            item.GetUncommittedEvents()
                .ToList()
                .ForEach(e => Bus.RaiseEvent(e));
            item.ClearUncommittedEvents();
        }

        public T GetById<T>(Guid id) where T : IAggregate
        {
            return (T) aggregates[id];
        }
    }
}
