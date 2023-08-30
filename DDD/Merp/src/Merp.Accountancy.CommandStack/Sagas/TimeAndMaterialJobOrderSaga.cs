using Merp.Accountancy.CommandStack.Commands;
using Merp.Accountancy.CommandStack.Events;
using Merp.Accountancy.CommandStack.Model;
using Merp.Accountancy.CommandStack.Services;
using Merp.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merp.Accountancy.CommandStack.Sagas
{
    public class TimeAndMaterialJobOrderSaga : Saga,
        IAmStartedBy<RegisterTimeAndMaterialJobOrderCommand>,
        IHandleMessage<ExtendTimeAndMaterialJobOrderCommand>,
        IHandleMessage<MarkTimeAndMaterialJobOrderAsCompletedCommand>
    {
        public IJobOrderNumberGenerator JobOrderNumberGenerator { get; private set; }

        public TimeAndMaterialJobOrderSaga(IBus bus, IEventStore eventStore, IRepository repository, IJobOrderNumberGenerator jobOrderNumberGenerator)
            : base(bus, eventStore, repository)
        {
            if(jobOrderNumberGenerator==null)
            {
                throw new ArgumentNullException("jobOrderNumberGenerator");
            }
            JobOrderNumberGenerator = jobOrderNumberGenerator;
        }

        public void Handle(RegisterTimeAndMaterialJobOrderCommand message)
        {
            var jobOrder = TimeAndMaterialJobOrder.Factory.CreateNewInstance(
                JobOrderNumberGenerator,
                message.CustomerId,
                message.CustomerName,
                message.ManagerId,
                message.ManagerName,
                message.Value,
                message.DateOfStart,
                message.DateOfExpiration,
                message.JobOrderName,
                message.PurchaseOrderNumber,
                message.Description
                );
            this.Repository.Save(jobOrder);
        }

        public void Handle(ExtendTimeAndMaterialJobOrderCommand message)
        {
            var jobOrder = Repository.GetById<TimeAndMaterialJobOrder>(message.JobOrderId);
            jobOrder.Extend(message.NewDateOfExpiration, message.Value);
            Repository.Save(jobOrder);
        }

        public void Handle(MarkTimeAndMaterialJobOrderAsCompletedCommand message)
        {
            var jobOrder = Repository.GetById<TimeAndMaterialJobOrder>(message.JobOrderId);
            jobOrder.MarkAsCompleted(message.DateOfCompletion);
            Repository.Save(jobOrder);
        }
    }
}
