using Merp.Accountancy.CommandStack.Commands;
using Merp.Accountancy.CommandStack.Model;
using Merp.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merp.Accountancy.CommandStack.Handlers
{
    public class JobOrderHandler : 
        IHandleMessage<AssociateIncomingInvoiceToJobOrderCommand>,
        IHandleMessage<AssociateOutgoingInvoiceToJobOrderCommand>
    {
        public IBus Bus { get; private set; }
        public IEventStore EventStore { get; private set; }
        public IRepository Repository { get; private set; }

        public JobOrderHandler(IBus bus, IEventStore eventStore, IRepository repository)
        {
            if (bus == null)
            {
                throw new ArgumentNullException("bus");
            }
            if (eventStore == null)
            {
                throw new ArgumentNullException("eventStore");
            }
            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }
            Bus = bus;
            EventStore = eventStore;
            Repository = repository;
        }

        public void Handle(AssociateIncomingInvoiceToJobOrderCommand message)
        {
            var jobOrder = Repository.GetById<JobOrder>(message.JobOrderId);
            jobOrder.AssociateIncomingInvoice(EventStore, message.InvoiceId);
        }

        public void Handle(AssociateOutgoingInvoiceToJobOrderCommand message)
        {
            var jobOrder = Repository.GetById<JobOrder>(message.JobOrderId);
            jobOrder.AssociateOutgoingInvoice(EventStore, message.InvoiceId);
        }
    }
}
