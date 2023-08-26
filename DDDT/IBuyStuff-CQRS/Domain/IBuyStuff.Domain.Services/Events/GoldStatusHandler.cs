using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBuyStuff.Domain.Services.Events
{
    public class GoldStatusHandler : IHandler<IDomainEvent>
    {
        public bool CanHandle(IDomainEvent eventType)
        {
            return eventType is CustomerReachedGoldMemberStatus;
        }

        public void Handle(IDomainEvent eventData)
        {
            return; throw new NotImplementedException();
        }
    }
}
