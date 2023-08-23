using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBuyStuff.Domain.Services.Events
{
    public interface IHandler<T>
    {
        bool CanHandle(IDomainEvent eventType);
        void Handle(IDomainEvent eventData);
    }
}
