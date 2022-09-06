using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackGroundMediatR
{
    internal class EventSecondHandler : INotificationHandler<TimedNotification>
    {
        private readonly TransientService _service;
        public EventSecondHandler(TransientService  service)
        {
            _service = service;
        }
        public Task Handle(TimedNotification notification, CancellationToken cancellationToken)
        {
            Console.WriteLine(_service.Id);
            return Task.CompletedTask;
        }
    }
}
