using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.SeedWork
{
    public class DomainEventBase : IDomainEvent
    {
        public DomainEventBase()
        {
            this.OccurredOn = DateTime.Now;
        }
        public DateTime OccurredOn { get; }
    }
}
