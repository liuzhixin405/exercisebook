using Mediator.Sample.Model;
using MediatR;
using System.Runtime.CompilerServices;

namespace Mediator.Sample.Handler
{
    public class MyBehaviorDo : IStreamPipelineBehavior<StreamPing, Pong>
    {
        public async IAsyncEnumerable<Pong> Handle(StreamPing request, [EnumeratorCancellation] CancellationToken cancellationToken, StreamHandlerDelegate<Pong> next)
        {
            yield return new Pong { Message = "Start behaving..." };

            await foreach (var item in next().WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                yield return item;
            }

            yield return new Pong { Message = "...Ready behaving" };
        }
    }
}
