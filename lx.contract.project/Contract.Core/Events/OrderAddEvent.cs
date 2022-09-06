using Contract.Core.Entities;
using Contract.Core.SharedKernel;
using Contract.SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contract.Core.Events
{
    public class OrderAddEvent:DomainEventBase
    {
        public ToDoItem NewItem { get; set; }
        public Order Order { get; set; }
        public OrderAddEvent(Order order,ToDoItem toDoItem)
        {
            NewItem = toDoItem;
            Order = order;
        }
    }
}
