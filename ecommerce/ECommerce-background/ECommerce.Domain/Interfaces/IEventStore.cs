using ECommerce.Domain.Events;

namespace ECommerce.Domain.Interfaces
{
    public interface IEventStore
    {
        Task SaveAsync<T>(T @event) where T : BaseEvent;
        Task<IEnumerable<T>> GetEventsAsync<T>(Guid aggregateId) where T : BaseEvent;
    }
}