using Contract.Core.Events;
using Contract.Core.SharedKernel;
using Contract.SharedKernel;
using Contract.SharedKernel.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contract.Core.Entities
{
    public class ToDoItem:EntityBase, IAggregateRoot
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsDone { get; private set; } = false;
        public void MarkComplete()
        {
            if (!IsDone)
            {
                IsDone = true;
                RegisterDomainEvent(new ToDoItemCompletedEvent(this));
            }
        }
        public override string ToString()
        {
            string status = IsDone ? "Done!" : "Not done.";
            return $"{Id}: Status: {status} - {Title} - {Description}";
        }
    }
}
