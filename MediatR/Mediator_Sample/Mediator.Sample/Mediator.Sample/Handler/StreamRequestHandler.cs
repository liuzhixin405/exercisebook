using Mediator.Sample.Model;
using MediatR;
using System.Runtime.CompilerServices;

namespace Mediator.Sample.Handler
{
    public class StreamRequestHandler : IStreamRequestHandler<StreamPing, Pong>
    {
        public async IAsyncEnumerable<Pong> Handle(StreamPing request, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            yield return await Task.Run(() => new Pong { Message = "hello " }, cancellationToken);
            yield return await Task.Run(() => new Pong { Message = "world" }, cancellationToken);
        }
    }
}
