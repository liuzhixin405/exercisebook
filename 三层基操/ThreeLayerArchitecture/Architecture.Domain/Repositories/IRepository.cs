using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Architecture.Domain.Repositories
{
    public interface IRepository<TAggregate>
    {
        IList<TAggregate> FindAll();
        bool Add(TAggregate aggregate);
        bool Save(TAggregate aggregate);
        bool Delete(TAggregate aggregate);
    }
}
