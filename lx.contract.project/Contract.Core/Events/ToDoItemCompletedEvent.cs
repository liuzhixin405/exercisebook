using Contract.Core.Entities;
using Contract.Core.SharedKernel;
using Contract.SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contract.Core.Events
{
    public class ToDoItemCompletedEvent : DomainEventBase
    {
        public ToDoItem CompletedItem { get; set; }

        public ToDoItemCompletedEvent(ToDoItem completedItem)
        {
            CompletedItem = completedItem;
        }
    }
}
