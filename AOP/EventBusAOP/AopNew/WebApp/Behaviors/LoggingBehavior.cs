using Common.Bus.Core;

namespace WebApp.Behaviors
{
    public class LoggingBehavior<TCommand, TResult> : ICommandPipelineBehavior<TCommand, TResult>
    where TCommand : ICommand<TResult>
    {
        public async Task<TResult> Handle(TCommand command, Func<TCommand, Task<TResult>> next, CancellationToken ct = default)
        {
            Console.WriteLine($"[LOG] Handling {typeof(TCommand).Name}...");
            var result = await next(command);
            Console.WriteLine($"[LOG] Handled {typeof(TCommand).Name}");
            return result;
        }
    }

}
