using Autofac;
using MediatR;
using Project.Application.Configuration.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.Processing
{
    public class CommandsExecutor
    {
        public static async Task Execute(ICommand command)
        {
            using (var scope = CompositionRoot.BeginLifetimeScope())
            {
                var mediator = scope.Resolve<IMediator>();
                await mediator.Send(command);
            }
        }

        public static async Task<TResult> Execute<TResult>(ICommand<TResult> command)
        {
            using (var scope = CompositionRoot.BeginLifetimeScope())
            {
                var mediator = scope.Resolve<IMediator>();
                return await mediator.Send(command);
            }
        }
    }
}
