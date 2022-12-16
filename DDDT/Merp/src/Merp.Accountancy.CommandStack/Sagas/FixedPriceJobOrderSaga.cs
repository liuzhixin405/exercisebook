using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merp.Infrastructure;
using Merp.Accountancy.CommandStack.Commands;
using Merp.Accountancy.CommandStack.Model;
using Merp.Accountancy.CommandStack.Events;
using Merp.Accountancy.CommandStack.Services;


namespace Merp.Accountancy.CommandStack.Sagas
{
    public sealed class FixedPriceJobOrderSaga : Saga,
        IAmStartedBy<RegisterFixedPriceJobOrderCommand>,
        IHandleMessage<ExtendFixedPriceJobOrderCommand>,
        IHandleMessage<MarkFixedPriceJobOrderAsCompletedCommand>
    {
        public IJobOrderNumberGenerator JobOrderNumberGenerator { get; private set; }

        public FixedPriceJobOrderSaga(IBus bus, IEventStore eventStore, IRepository repository, IJobOrderNumberGenerator jobOrderNumberGenerator)
            : base(bus, eventStore, repository)
        {
            if(jobOrderNumberGenerator==null)
            {
                throw new ArgumentNullException("jobOrderNumberGenerator");
            }
            JobOrderNumberGenerator = jobOrderNumberGenerator;
        }

        public void Handle(RegisterFixedPriceJobOrderCommand message)
        {
            var jobOrder = FixedPriceJobOrder.Factory.CreateNewInstance(
                JobOrderNumberGenerator,
                message.CustomerId,
                message.CustomerName,
                message.ManagerId,
                message.ManagerName,
                message.Price, 
                message.DateOfStart, 
                message.DueDate,
                message.JobOrderName,
                message.PurchaseOrderNumber,
                message.Description
                ); 
            this.Repository.Save(jobOrder);
        }

        public void Handle(ExtendFixedPriceJobOrderCommand message)
        {
            var jobOrder = Repository.GetById<FixedPriceJobOrder>(message.JobOrderId);
            jobOrder.Extend(message.NewDueDate, message.Price);
            Repository.Save(jobOrder);
        }

        public void Handle(MarkFixedPriceJobOrderAsCompletedCommand message)
        {
            var jobOrder = Repository.GetById<FixedPriceJobOrder>(message.JobOrderId);
            jobOrder.MarkAsCompleted(message.DateOfCompletion);
            Repository.Save(jobOrder);
        }
    }
}
