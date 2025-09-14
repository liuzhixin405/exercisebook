using Common.Bus.Core;
using WebApp.Commands;

namespace WebApp.Behaviors
{
    public class ValidationBehavior<TCommand, TResult> : ICommandPipelineBehavior<TCommand, TResult>
     where TCommand : ICommand<TResult>
    {
        public async Task<TResult> Handle(TCommand command, Func<Task<TResult>> next, CancellationToken ct = default)
        {
            Console.WriteLine("[VALIDATION] Validating...");

            //validation

            if (command is ProcessOrderCommand po && po.Quantity <= 0)
                throw new ArgumentException("Order quantity must be greater than zero!");
            return await next();
        }
    }

}
