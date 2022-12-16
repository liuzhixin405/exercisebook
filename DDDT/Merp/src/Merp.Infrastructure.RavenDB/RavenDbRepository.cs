using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merp.Infrastructure;
using Raven.Client.Embedded;

namespace Merp.Infrastructure.RavenDB
{
    public class RavenDbRepository : IRepository
    {
        private static EmbeddableDocumentStore DocumentStore { get; set; }
        static RavenDbRepository()
        {
            DocumentStore = new EmbeddableDocumentStore
            {
                ConnectionStringName = "DocumentStore"
                //UseEmbeddedHttpServer = true
            };
            DocumentStore.Conventions.AllowQueriesOnId = true; //Fix this
            DocumentStore.Initialize();
        }

        public IBus Bus { get; private set; }

        public RavenDbRepository(IBus bus)
        {
            if (bus == null)
            {
                throw new ArgumentNullException("bus");
            }
            Bus = bus;
        }

        public T GetById<T>(Guid id) where T : IAggregate
        {
            //session.Query().Where(x=>x.Id == "FixedPriceJobOrders/ac16d75e-3423-41cf-a5a9-70fb4b48088b");
            using (var session = DocumentStore.OpenSession())
            {
                var item = (from i in session.Query<T>()
                            where i.Id == id
                            select i).Single();
                return item;
            }        
        }

        public void Save<T>(T item) where T : IAggregate
        {
            PersistAggregate(item);
            ManageUncommittedEvents(item);
        }
        private void PersistAggregate<T>(T item) where T : IAggregate
        {
            using (var session = DocumentStore.OpenSession())
            {
                session.Store(item);
                session.SaveChanges();
            }
        }

        private void ManageUncommittedEvents<T>(T item) where T : IAggregate
        {
            item.GetUncommittedEvents()
                .ToList()
                .ForEach(e => Bus.RaiseEvent(e));
            item.ClearUncommittedEvents();
        }
    }
}
