using MediatR;
using System;

namespace Contract.SharedKernel
{
    public abstract class DomainEventBase:INotification
    {
        public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
    }
}
