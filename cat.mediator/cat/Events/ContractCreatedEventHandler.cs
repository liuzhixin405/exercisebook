using MediatR;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

namespace cat.Events
{
    public class ContractCreatedEventHandler : INotificationHandler<ContractCreatedEvent>
    {
        private readonly ILogger<ContractCreatedEventHandler> _logger;
        public ContractCreatedEventHandler(ILogger<ContractCreatedEventHandler> logger)
        {
            _logger = logger;
        }
        public Task Handle(ContractCreatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"contract {notification.ContractName} created !");
            return Task.CompletedTask;
        }
    }
}
