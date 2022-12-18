using MediatR;
using Project.Application.Configuration.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Application.Customers.IntegrationHandlers
{
    public class CustomerRegisteredNotificationHandler: INotificationHandler<CustomerRegisteredNotification>
    {
        private readonly ICommandsScheduler _commandsScheduler;

        public CustomerRegisteredNotificationHandler(
            ICommandsScheduler commandsScheduler)
        {
            _commandsScheduler = commandsScheduler;
        }

        public async Task Handle(CustomerRegisteredNotification notification, CancellationToken cancellationToken)
        {
            // Send welcome e-mail message...

            await this._commandsScheduler.EnqueueAsync(new MarkCustomerAsWelcomedCommand(
                Guid.NewGuid(),
                notification.CustomerId));
        }
    }
}
