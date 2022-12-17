using Autofac;
using Autofac.Core;
using MediatR;
using Project.Application.Configuration.DomainEvents;
using Project.Domain.SeedWork;
using Project.Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.Processing
{
    public class DomainEventsDispatcher : IDomainEventsDispatcher
    {
        private readonly IMediator mediator;
        private readonly ILifetimeScope scope;
        private readonly OrdersContext context;
        public DomainEventsDispatcher(IMediator mediator, ILifetimeScope scope, OrdersContext context)
        {
            this.mediator = mediator;
            this.scope = scope;
            this.context = context;
        }
        public async Task DispatchEventsAsync()
        {
            var domainEntities = context.ChangeTracker.Entries<Entity>().Where(x=>x.Entity.DomainEvents!=null &&x.Entity.DomainEvents.Any()).ToList();
            
            var domainEvents = domainEntities.SelectMany(x=>x.Entity.DomainEvents).ToList();

            var domainEventNotifications = new List<IDomainEventNotification<IDomainEvent>>();
            foreach (var domainEvent in domainEvents)
            {
                Type domainEventNotificationType = typeof(IDomainEventNotification<>);
                var domainNotificationWithGenericType = domainEventNotificationType.MakeGenericType(domainEvent.GetType());
                var domainNotification = scope.ResolveOptional(domainNotificationWithGenericType, new List<Parameter>
                {
                    new NamedParameter("domainEvent",domainEvent)
                });
                if (domainNotification != null)
                {
                    domainEventNotifications.Add(domainNotification as IDomainEventNotification<IDomainEvent>);
                }
            }

            domainEntities.ForEach(entity=>entity.Entity.ClearDomainEvents());

            var tasks = domainEvents.Select(async (domainEvent) =>
            {
                await mediator.Publish(domainEvent);
            });
            await Task.WhenAll(tasks);
        }
    }
}
