using Common.Bus.Core;

namespace WebApp.Behaviors
{
    public class TransactionBehavior<TCommand, TResult> : ICommandPipelineBehavior<TCommand, TResult>
     where TCommand : ICommand<TResult>
    {
        public async Task<TResult> Handle(TCommand command, Func<TCommand, Task<TResult>> next, CancellationToken ct = default)
        {
            Console.WriteLine("[TX] Begin Transaction");
            try
            {
                var result = await next(command);
                Console.WriteLine("[TX] Commit Transaction");
                return result;
            }
            catch
            {
                Console.WriteLine("[TX] Rollback Transaction");
                throw;
            }
        }
    }

}
