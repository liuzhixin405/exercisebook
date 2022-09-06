using MediatR;

namespace Mediator.Sample.Model
{
    public class Ping:IRequest<Pong>
    {
        public String Message { get; set; }
    }
}
