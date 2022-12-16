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
    public class TimeAndMaterialJobOrder : JobOrder
    {
        public decimal? Value { get; private set; }

        public DateTime? DateOfExpiration { get; private set; }

        protected TimeAndMaterialJobOrder()
        {
            
        }

        public void Extend(DateTime? newDateOfExpiration, decimal? value)
        {
            this.DateOfExpiration = newDateOfExpiration;
            this.Value = value;

            var @event = new TimeAndMaterialJobOrderExtendedEvent(
                this.Id,
                this.DateOfExpiration,
                this.Value
            );
            RaiseEvent(@event);
        }

        public void MarkAsCompleted(DateTime dateOfCompletion)
        {
            if (this.DateOfStart > dateOfCompletion)
            {
                throw new ArgumentException("The date of completion cannot precede the date of start.", "dateOfCompletion");
            }
            if (this.IsCompleted)
            {
                throw new InvalidOperationException("The Job Order has already been marked as completed");
            }
            this.DateOfCompletion = dateOfCompletion;
            this.IsCompleted = true;
            var @event = new TimeAndMaterialJobOrderCompletedEvent(
                this.Id,
                dateOfCompletion
            );
            RaiseEvent(@event);
        }

        public class Factory
        {
            public static TimeAndMaterialJobOrder CreateNewInstance(IJobOrderNumberGenerator jobOrderNumberGenerator, Guid customerId, string customerName, Guid managerId, string managerName, decimal? value, DateTime dateOfStart, DateTime? dateOfExpiration, string name, string purchaseOrderNumber, string description)
            {
                var id = Guid.NewGuid();
                var jobOrder = new TimeAndMaterialJobOrder() 
                {
                    Id = id,
                    Customer = new CustomerInfo(customerId, customerName),
                    Manager = new ManagerInfo(managerId, managerName),
                    Value = value,
                    DateOfStart= dateOfStart,
                    DateOfExpiration=dateOfExpiration,
                    Name = name,
                    Number = jobOrderNumberGenerator.Generate(), 
                    IsCompleted = false,
                    PurchaseOrderNumber = purchaseOrderNumber,
                    Description = description
                };
                var @event = new TimeAndMaterialJobOrderRegisteredEvent(
                    jobOrder.Id,
                    jobOrder.Customer.Id,
                    jobOrder.Customer.Name,
                    jobOrder.Manager.Id,
                    jobOrder.Manager.Name,
                    jobOrder.Value,
                    jobOrder.DateOfStart,
                    jobOrder.DateOfExpiration,
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
