using Merp.Accountancy.CommandStack.Events;
using Merp.Accountancy.CommandStack.Services;
using Merp.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merp.Accountancy.CommandStack.Model
{
    public class FixedPriceJobOrder : JobOrder
    {
        public decimal Price { get; private set; }
        public DateTime DueDate { get; private set; }

        protected FixedPriceJobOrder()
        {
            
        }

        public void Extend(DateTime newDueDate, decimal price)
        {
            this.DueDate = newDueDate;
            this.Price = price;

            var @event = new FixedPriceJobOrderExtendedEvent(
                this.Id, 
                this.DueDate,
                this.Price
            );
            RaiseEvent(@event);
        }

        public void MarkAsCompleted(DateTime dateOfCompletion)
        {
            if(this.DateOfStart > dateOfCompletion)
            {
                throw new ArgumentException("The date of completion cannot precede the date of start.", "dateOfCompletion");
            }
            if(this.IsCompleted)
            {
                throw new InvalidOperationException("The Job Order has already been marked as completed");
            }
            this.DateOfCompletion = dateOfCompletion;
            this.IsCompleted = true;
            var @event = new FixedPriceJobOrderCompletedEvent(
                this.Id,
                dateOfCompletion
            );
            RaiseEvent(@event);
        }

        public class Factory
        {
            public static FixedPriceJobOrder CreateNewInstance(IJobOrderNumberGenerator jobOrderNumberGenerator, Guid customerId, string customerName, Guid managerId, string managerName, decimal price, DateTime dateOfStart, DateTime dueDate, string name, string purchaseOrderNumber, string description)
            {
                var id = Guid.NewGuid();
                var jobOrder = new FixedPriceJobOrder() 
                {
                    Id = id,
                    Customer = new CustomerInfo(customerId, customerName),
                    Manager = new ManagerInfo(managerId, managerName),
                    Price = price,
                    DateOfStart= dateOfStart,
                    DueDate=dueDate,
                    Name = name,
                    Number = jobOrderNumberGenerator.Generate(), 
                    IsCompleted = false,
                    PurchaseOrderNumber = purchaseOrderNumber,
                    Description = description
                };
                var @event = new FixedPriceJobOrderRegisteredEvent(
                    jobOrder.Id,
                    jobOrder.Customer.Id,
                    jobOrder.Customer.Name,
                    jobOrder.Manager.Id,
                    jobOrder.Manager.Name,
                    jobOrder.Price,
                    jobOrder.DateOfStart,
                    jobOrder.DueDate,
                    jobOrder.Name,
                    jobOrder.Number,
                    jobOrder.PurchaseOrderNumber,
                    jobOrder.Description
                    );
                jobOrder.RaiseEvent(@event);
                return jobOrder;
            }
        }
    }
}
