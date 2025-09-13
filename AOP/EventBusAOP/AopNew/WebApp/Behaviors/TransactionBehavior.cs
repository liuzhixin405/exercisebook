using Common.Bus.Core;

namespace WebApp.Filters
{
    public class TransactionBehavior<TCommand, TResult> : ICommandPipelineBehavior<TCommand, TResult>
     where TCommand : ICommand<TResult>
    {
        public async Task<TResult> Handle(TCommand command, Func<Task<TResult>> next, CancellationToken ct = default)
        {
            Console.WriteLine("[TX] Begin Transaction");
            try
            {
                var result = await next();
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
