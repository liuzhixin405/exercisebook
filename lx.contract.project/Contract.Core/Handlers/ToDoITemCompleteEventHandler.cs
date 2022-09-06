using Contract.Core.Events;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.Core.Handlers
{
    internal class ToDoITemCompleteEventHandler : NotificationHandler<ToDoItemCompletedEvent>
    {
        protected override void Handle(ToDoItemCompletedEvent notification)
        {
            //TODO
        }
    }
}
