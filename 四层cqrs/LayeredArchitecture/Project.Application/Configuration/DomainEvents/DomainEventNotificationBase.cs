using Project.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Project.Application.Configuration.DomainEvents
{
    public class DomainEventNotificationBase<T> : IDomainEventNotification<T> where T : IDomainEvent
    {
        [JsonIgnore]
        public T DomainEvent { get; }
       public Guid Id { get; }

        public DomainEventNotificationBase(T domainEvent)
        {
            this.Id=Guid.NewGuid();
            this.DomainEvent=domainEvent;
        }
    }
}
