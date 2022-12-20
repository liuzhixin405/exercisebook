using Architecture.Domain.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Architecture.Domain.Repositories
{
    public interface ISubscriberRepository : IRepository<Subscriber>
    {
        int Count();
    }
}
