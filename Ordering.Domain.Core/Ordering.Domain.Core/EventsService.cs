using Ordering.Domain.Core.EventBus;
using Ordering.Domain.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.Core
{
    internal class EventsService<TType, TKey> : IEventService<TType, TKey> where TType : class, IAggregateRoot<TKey>
    {
        private readonly IEventProducer<TType, TKey> _eventProducer;
        private readonly IEventRepository<TType, TKey> _eventRepository;
        public EventsService(IEventProducer<TType, TKey> eventProducer,
            IEventRepository<TType, TKey> eventRepository)
        {
            _eventProducer = eventProducer;
            _eventRepository = eventRepository;
        }
        public async Task PersistAsync(TType aggregateRoot)
        {
            await _eventProducer.DispatchAsync(aggregateRoot);
            await _eventRepository.AppendAsync(aggregateRoot);
            aggregateRoot.ClearEvents();
        }
    }
}
