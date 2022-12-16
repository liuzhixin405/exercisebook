using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merp.Infrastructure
{
    public interface IEventStore
    {
        IEnumerable<T> Find<T>(Func<T,bool> filter) where T : DomainEvent;

        void Save<T>(T @event) where T : DomainEvent;
    }
}
