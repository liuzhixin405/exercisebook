using MassTransit;
using OrderApi.Infrastructure;
using Shared.Contract.Events;
using System.Threading.Tasks;

namespace OrderApi.Integrations.Consumers
{
    public class CustomerCreatedEventConsumer : IConsumer<CustomerCreatedEvent>
    {
        private readonly OrderDbContext dbContext;

        public CustomerCreatedEventConsumer(OrderDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public Task Consume(ConsumeContext<CustomerCreatedEvent> context)
        {
            dbContext.Customers.Add(new Model.Customer()
            {
               Id = context.Message.CustomerId,
               CustomerName = context.Message.FullName
            });
            return dbContext.SaveChangesAsync();
        }
    }
}
