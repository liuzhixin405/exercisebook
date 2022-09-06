using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackGroundMediatR
{
    internal class EveryFiveSecondHandler : INotificationHandler<TimedNotification>
    {
        public Task Handle(TimedNotification notification, CancellationToken cancellationToken)
        {
            if(notification.Time.Second % 5==0)
            Console.WriteLine(notification.Time.ToLongTimeString());
            return Task.CompletedTask;
        }
    }
}
