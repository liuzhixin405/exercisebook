using System.Collections.Generic;
using System.Linq;
using IBuyStuff.Domain.Misc;
using IBuyStuff.Domain.Repositories;
using IBuyStuff.Persistence.Facade;

namespace IBuyStuff.Persistence.Repositories
{
    public class SubscriberRepository : ISubscriberRepository
    {
        private readonly CommandModelDatabase _database;
        public SubscriberRepository(CommandModelDatabase database)
        {
                _database = database;
        }
        public int Count()
        {
         
                return (from s in _database.Subscribers select s).Count();
            
        }

        #region IRepository
        public IList<Subscriber> FindAll()
        {
          return  _database.Subscribers.ToList();
        }

        public bool Add(Subscriber aggregate)
        {

            _database.Subscribers.Add(aggregate);
                return _database.SaveChanges() > 0;
            
        }

        public bool Save(Subscriber aggregate)
        {
            throw new System.NotImplementedException();
        }

        public bool Delete(Subscriber aggregate)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}