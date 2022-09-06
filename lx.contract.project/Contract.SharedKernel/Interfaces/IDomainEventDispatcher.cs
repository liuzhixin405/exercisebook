using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Contract.SharedKernel.Interfaces
{
    public interface IDomainEventDispatcher
    {
        Task DispatchAndClearEvents(IEnumerable<EntityBase> entitiesWithEvents);
    }
}
