using Merp.Accountancy.CommandStack.Events;
using Merp.Accountancy.QueryStack.Model;
using Merp.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merp.Accountancy.QueryStack.Denormalizers
{
    public class FixedPriceJobOrderDenormalizer : 
        IHandleMessage<FixedPriceJobOrderRegisteredEvent>,
        IHandleMessage<FixedPriceJobOrderExtendedEvent>,
        IHandleMessage<FixedPriceJobOrderCompletedEvent>
    {
        public void Handle(FixedPriceJobOrderRegisteredEvent message)
        {
            var fixedPriceJobOrder = new FixedPriceJobOrder();
            fixedPriceJobOrder.OriginalId = message.JobOrderId;
            fixedPriceJobOrder.CustomerId = message.CustomerId;
            fixedPriceJobOrder.CustomerName = message.CustomerName;
            fixedPriceJobOrder.Description = message.Description;
            fixedPriceJobOrder.ManagerId = message.ManagerId;
            fixedPriceJobOrder.ManagerName = message.ManagerName; 
            fixedPriceJobOrder.DateOfStart = message.DateOfStart;
            fixedPriceJobOrder.DueDate = message.DueDate;
            fixedPriceJobOrder.Name = message.JobOrderName;
            fixedPriceJobOrder.Number = message.JobOrderNumber;
            fixedPriceJobOrder.Price = message.Price;
            fixedPriceJobOrder.PurchaseOrderNumber = message.PurchaseOrderNumber;
            fixedPriceJobOrder.IsCompleted = false;
            fixedPriceJobOrder.IsTimeAndMaterial = false;
            fixedPriceJobOrder.IsFixedPrice = true; 

            using(var db = new AccountancyContext())
            {
                db.JobOrders.Add(fixedPriceJobOrder);
                db.SaveChanges();
            }
        }

        public void Handle(FixedPriceJobOrderExtendedEvent message)
        {
            using(var db = new AccountancyContext())
            {
                var jobOrder = db.JobOrders.OfType<FixedPriceJobOrder>().Where(jo => jo.OriginalId == message.JobOrderId).Single();
                jobOrder.DueDate = message.NewDueDate;
                jobOrder.Price = message.Price;
                db.SaveChanges();
            }
        }

        public void Handle(FixedPriceJobOrderCompletedEvent message)
        {
            using (var db = new AccountancyContext())
            {
                var jobOrder = db.JobOrders.OfType<FixedPriceJobOrder>().Where(jo => jo.OriginalId == message.JobOrderId).Single();
                jobOrder.DateOfCompletion = message.DateOfCompletion;
                jobOrder.IsCompleted = true;
                db.SaveChanges();
            }
        }
    }
}
