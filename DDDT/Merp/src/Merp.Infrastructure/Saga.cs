using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Merp.Infrastructure
{
    public abstract class Saga
    {
        public IBus Bus { get; private set; }
        public IEventStore EventStore { get; private set; }

        public IRepository Repository { get; private set; }

        public Saga(IBus bus, IEventStore eventStore, IRepository repository)
        {
            if (bus == null)
            {
                throw new ArgumentNullException("bus");
            }
            if (eventStore == null)
            {
                throw new ArgumentNullException("eventStore");
            }
            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }
            Bus = bus;
            EventStore = eventStore;
            Repository = repository;
        }
    }
}
