using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
namespace Common.Bus
{
    public class CommandBus : ICommandBus
    {
        private readonly IServiceProvider _provider;
        public CommandBus(IServiceProvider serviceProvider)
        {
            _provider = serviceProvider;
        }
        public async Task<TResult> SendAsync<TCommand, TResult>(TCommand command, CancellationToken ct = default) where TCommand : ICommand<TResult>
        {
           var handler =(ICommandHandler<TCommand, TResult>)_provider.GetService(typeof(ICommandHandler<TCommand, TResult>))??throw new Exception($"No handler registered for {typeof(TCommand).Name}");

            var behaviors = _provider.GetServices<ICommandPipelineBehavior<TCommand, TResult>>().ToList();

            Func<Task<TResult>> pipeline = ()=>handler.HandleAsync(command, ct);

            foreach(var behavior in behaviors.AsEnumerable().Reverse())
            {
                var next = pipeline;
                pipeline = () => behavior.Handle(command, next, ct);
            }
            return await pipeline();

        }
    }
}
