using CqrsWithEs.Domain.Base;
using MediatR;
using System.Collections.Concurrent;

namespace CqrsWithEs.DataAccess
{
    public interface IEventStore
    {
        IMediator Bus { get; set; }
        void SaveEvents(Guid aggregate, IEnumerable<Event> events, int expectedVersion);
        List<Event> GetEventsForAggregate(Guid aggregate);
    }
    public class EventStore : IEventStore
    {
        public IMediator Bus { get; set; }
        private struct EventDescriptor
        {
            public readonly Event EventData;
            public readonly Guid Id;
            public readonly int Version;
            public EventDescriptor( Guid id, Event eventData, int version)
            {
                this.EventData = eventData;
                this.Id = id;
                this.Version = version;
            }
        }
        public EventStore(){ }
        private readonly IDictionary<Guid,List<EventDescriptor>> current = new ConcurrentDictionary<Guid,List<EventDescriptor>>();
        public void SaveEvents(Guid aggregateId,IEnumerable<Event> events,int expectedVersion)
        {
            List<EventDescriptor> eventDescriptors;
            if (!current.TryGetValue(aggregateId, out eventDescriptors))
            {
                eventDescriptors = new List<EventDescriptor>();
                current.Add(aggregateId, eventDescriptors);
            }
            else if (eventDescriptors[eventDescriptors.Count - 1].Version != expectedVersion && expectedVersion != -1)
            {
                throw new ConcurrencyException();
            }
            var i = expectedVersion;
            foreach (var @event in events)
            {
                i++;
                @event.Version = i;

                // push event to the event descriptors list for current aggregate
                eventDescriptors.Add(new EventDescriptor(aggregateId, @event, i));

                // publish current event to the bus for further processing by subscribers
                Bus.Publish(@event);
            }
        }
        public List<Event> GetEventsForAggregate(Guid aggregateId)
        {
            List<EventDescriptor> eventDescriptors;
            if (!current.TryGetValue(aggregateId, out eventDescriptors))
            {
                throw new AggregateNotFoundException();
            }

            return eventDescriptors.Select(desc => desc.EventData).ToList();
        }
    }

    public class AggregateNotFoundException : Exception
    {
    }

    public class ConcurrencyException : Exception
    {
    }
}
