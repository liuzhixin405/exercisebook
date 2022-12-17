using Project.Domain.SeedWork;
using Project.Infrastructure.Database;
using Project.Infrastructure.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.Domain
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly OrdersContext ordersContext;
        private readonly IDomainEventsDispatcher domainEventsDispatcher;
        public UnitOfWork(OrdersContext ordersContext, IDomainEventsDispatcher domainEventsDispatcher)
        {
            this.ordersContext = ordersContext;
            this.domainEventsDispatcher = domainEventsDispatcher;
        }
        public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
        {
            await domainEventsDispatcher.DispatchEventsAsync();
            return await ordersContext.SaveChangesAsync(cancellationToken);
        }
    }
}
