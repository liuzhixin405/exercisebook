using MediatR;

namespace Mediator.Sample.Model
{
    public class StreamPing:IStreamRequest<Pong>
    {
        public string StreamMessage { get; set; }
    }
}
