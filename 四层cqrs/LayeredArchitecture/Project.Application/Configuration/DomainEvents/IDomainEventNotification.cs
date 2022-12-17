using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Application.Configuration.DomainEvents
{
    public interface IDomainEventNotification<out TEventType>: IDomainEventNotification
    {
        TEventType DomainEvent { get; }
    }

    public interface IDomainEventNotification:INotification
    {
        Guid Id { get; }
    }
}
