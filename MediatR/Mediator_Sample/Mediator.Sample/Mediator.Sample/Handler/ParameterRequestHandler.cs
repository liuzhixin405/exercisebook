using Mediator.Sample.Model;
using MediatR;

namespace Mediator.Sample.Handler
{
    public class ParameterRequestHandler : IRequestHandler<PingSource, Pong>
    {
        public Task<Pong> Handle(PingSource request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new Pong { Message = $"hello ,sender =>{request.Message}" });
        }
    }
}
