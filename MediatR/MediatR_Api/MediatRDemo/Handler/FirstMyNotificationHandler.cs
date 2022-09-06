using MediatR;
using MediatRDemo.Model;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace MediatRDemo.Handler
{
    public class FirstMyNotificationHandler : INotificationHandler<MyNotificationCommand>
    {
        public async Task Handle(MyNotificationCommand notification, CancellationToken cancellationToken)
        {
            //针对广播的内容做进一步处理
            Debug.WriteLineIf(!string.IsNullOrEmpty(notification.Message), $"First notification handler:{notification.Message}");
        }
    }

    public class SecondMyNotificationHandler : INotificationHandler<MyNotificationCommand>
    {
        public async Task Handle(MyNotificationCommand notification, CancellationToken cancellationToken)
        {
            //针对广播的内容做进一步处理
            Debug.WriteLineIf(!string.IsNullOrEmpty(notification.Message), $"Second notification handler:{notification.Message}");
        }
    }
}
