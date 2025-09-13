using Common.Bus.Core;
using WebApp.Controllers;

namespace WebApp.Filters
{
    public class ValidationBehavior<TCommand, TResult> : ICommandPipelineBehavior<TCommand, TResult>
     where TCommand : ICommand<TResult>
    {
        public async Task<TResult> Handle(TCommand command, Func<Task<TResult>> next, CancellationToken ct = default)
        {
            Console.WriteLine("[VALIDATION] Validating...");

            //validation

            if (command is CreateOrderCommand co && co.Quantity <= 0)
                throw new ArgumentException("Order quantity must be greater than zero!");
            return await next();
        }
    }

}
