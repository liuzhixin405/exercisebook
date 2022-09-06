using CqrsLibrary.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CqrsLibrary.Events
{
    public interface IEventStore
    {
        void SaveEvents(Guid aggregateId,IEnumerable<Event> events,int expectedVersion);
        IEnumerable<Event> GetEventsForAggregate(Guid aggregateId);
    }

    public class EventStore : IEventStore
    {
        private readonly IEventPublisher publisher;
        private readonly IDictionary<Guid, List<EventDescriptor>> current = new Dictionary<Guid,List<EventDescriptor>>();
        private struct EventDescriptor
        {
            private readonly Event eventData;
            private readonly Guid id;
            private readonly int version;
            public EventDescriptor(Event eventData, Guid id, int version)
            {
                this.eventData = eventData;
                this.id= id;
                this.version= version;
            }

            public Event EventData => eventData;

            public Guid Id => id;

            public int Version => version;
        }
        public EventStore(IEventPublisher eventPublisher)
        {
            this.publisher = eventPublisher;
        }
        public IEnumerable<Event> GetEventsForAggregate(Guid aggregateId)
        {
            List<EventDescriptor> eventDescriptors;
            if(!current.TryGetValue(aggregateId, out eventDescriptors))
            {
                throw new AggregateNotFoundException();
            }
            return eventDescriptors.Select(e=>e.EventData).ToList();
        }

        public void SaveEvents(Guid aggregateId, IEnumerable<Event> events, int expectedVersion)
        {
            List<EventDescriptor> eventDescriptors;
            if(!current.TryGetValue(aggregateId, out eventDescriptors))
            {
                eventDescriptors = new List<EventDescriptor>();
                current.Add(aggregateId, eventDescriptors);
            }

            else if(eventDescriptors[eventDescriptors.Count - 1].Version!=expectedVersion && expectedVersion!=-1)
            {
                throw new ConcurrencyException();
            }

            var i = expectedVersion;
            foreach (var @event in events)
            {
                i++;
                @event.Version = i;
                eventDescriptors.Add(new EventDescriptor(@event, aggregateId, i));
                
                publisher.Publish(@event);
            }
        }
    }

    public class AggregateNotFoundException : Exception
    {
    }

    public class ConcurrencyException : Exception
    {
    }
}
