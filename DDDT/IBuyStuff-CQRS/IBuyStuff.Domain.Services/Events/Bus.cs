using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBuyStuff.Domain.Services.Events
{
    public class Bus
    {
        private static readonly IList<IHandler<IDomainEvent>> Handlers = new List<IHandler<IDomainEvent>>();
        public static void Register(IHandler<IDomainEvent> handler)
        {
            if (handler != null)
                Handlers.Add(handler);
        }
        public static void Raise<T>(T eventData) where T : IDomainEvent
        {
            foreach (var handler in Handlers)
            {
                if (handler.CanHandle(eventData))
                    handler.Handle(eventData);
            }
        }

        public void Test()
        {
            Bus.Register(new GoldStatusHandler());
            Bus.Raise(new CustomerReachedGoldMemberStatus());
        }
    }
}
