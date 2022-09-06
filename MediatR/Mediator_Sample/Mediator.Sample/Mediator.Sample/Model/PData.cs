using MediatR;

namespace Mediator.Sample.Model
{
    public class PData:INotification //一对多
    {
        public string Message { get; set; }
    }
}
