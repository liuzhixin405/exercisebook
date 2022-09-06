using CqrsLibrary.Commands;
using CqrsLibrary.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CqrsLibrary.Events
{
    public class FakeBus : ICommandSender, IEventPublisher
    {
        private readonly IDictionary<Type, List<Action<Message>>> routes = new Dictionary<Type, List<Action<Message>>>();
        public void RegisterHandler<T>(Action<T> handler) where T : Message
        {
            List<Action<Message>> handlers;
            if(!routes.TryGetValue(typeof(T),out handlers))
            {
                handlers = new List<Action<Message>>();
                routes.Add(typeof(T),handlers);
            }
            handlers.Add(x => handler((T)x));
        }
        public void Publish<T>(T @event) where T : Event
        {
            List<Action<Message>> handlers;
            if (!routes.TryGetValue(@event.GetType(), out handlers)) return;
            foreach (var handler in handlers)
            {
                var action = handler;
                Task.Factory.StartNew(() => action(@event));
            }
        }

        public void Send<T>(T command) where T : Command
        {
            List<Action<Message>> handlers;
            if (routes.TryGetValue(typeof(T), out handlers))
            {
                if (handlers.Count != 1) throw new InvalidOperationException("cannot send to more than one handler");
                handlers[0](command);
            }
            else
            {
                throw new InvalidOperationException("no handler registered");
            }
        }
    }

    public interface ICommandSender
    {
        void Send<T>(T command) where T : Command;

    }
    public interface IEventPublisher
    {
        void Publish<T>(T @event) where T : Event;
    }

    public interface Handles<T>
    {
        void Handle(T message);
    }
}
