using Mediator.Sample.Model;
using MediatR;

namespace Mediator.Sample.Handler
{
    public class MyPublishHandler : INotificationHandler<PData>
    {
        public Task Handle(PData notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"this=>{notification.Message}");
            return Task.CompletedTask;
        }
    }

    public class MyPublishHandlerNext : INotificationHandler<PData>
    {
        public Task Handle(PData notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"next=>{notification.Message}");
            return Task.CompletedTask;
        }
    }
}
