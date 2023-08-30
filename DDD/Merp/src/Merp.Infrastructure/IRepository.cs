using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merp.Infrastructure
{
    public interface IRepository 
    {
        T GetById<T>(Guid id) where T : IAggregate;
        void Save<T>(T item) where T : IAggregate;
    }
}
