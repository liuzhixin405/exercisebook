using MediatR;
using Microsoft.Extensions.Logging;
using Project.Application.Configuration;
using Project.Application.Configuration.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.Logging
{
    public class LoggingCommandHandlerDecorator<T> : ICommandHandler<T> where T:ICommand
    {
        private readonly ILogger logger;
        private readonly IExecutionContextAccessor executionContextAccessor;
        private readonly ICommandHandler<T> decorated;
        public LoggingCommandHandlerDecorator(ILogger logger, IExecutionContextAccessor executionContextAccessor, ICommandHandler<T> decorated)
        {
            this.logger = logger;
            this.executionContextAccessor = executionContextAccessor;
            this.decorated = decorated;
        }

        public async Task<Unit> Handle(T command, CancellationToken cancellationToken)
        {
            await
                Task.CompletedTask;
            return Unit.Value;
        }
    }
}
