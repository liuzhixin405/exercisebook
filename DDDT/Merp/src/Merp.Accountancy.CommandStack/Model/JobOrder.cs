using Merp.Accountancy.CommandStack.Events;
using Merp.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merp.Accountancy.CommandStack.Model
{
    public abstract class JobOrder : Aggregate
    {
        public CustomerInfo Customer { get; protected set; }
        public ManagerInfo Manager { get; protected set; }
        public string Name { get; protected set; }
        public string Number { get; protected set; }
        public DateTime DateOfStart { get; protected set; }
        public DateTime? DateOfCompletion { get; protected set; }
        public bool IsCompleted { get; protected set; }
        public string PurchaseOrderNumber { get; protected set; }
        public string Description { get; protected set; }
        public class CustomerInfo
        {
            public Guid Id { get; private set; }
            public string Name { get; private set; }

            public CustomerInfo(Guid id, string name)
            {
                if (id == Guid.Empty)
                {
                    throw new ArgumentException("Id cannot be empty", "id");
                }
                if(string.IsNullOrWhiteSpace(name))
                {
                    throw new ArgumentException("Name cannot be null or empty", "name");
                }
                Id = id;
                Name = name;
            }
        }

        public class ManagerInfo
        {
            public Guid Id { get; private set; }
            public string Name { get; private set; }

            public ManagerInfo(Guid id, string name)
            {
                if (id == Guid.Empty)
                {
                    throw new ArgumentException("Id cannot be empty", "id");
                }
                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new ArgumentException("Name cannot be null or empty", "name");
                }
                Id = id;
                Name = name;
            }
        }

        /// <summary>
        /// Associate an incoming invoice to the current Job Order
        /// </summary>
        /// <param name="eventStore">The event store</param>
        /// <param name="invoiceId">The Id of the Invoice to be associated to the current Job Order</param>
        /// <exception cref="InvalidOperationException">Thrown if the specified invoiceId refers to an invoice which has already been associated to a Job Order</exception>
        public void AssociateIncomingInvoice(IEventStore eventStore, Guid invoiceId)
        {
            var count = eventStore.Find<IncomingInvoiceAssociatedToJobOrderEvent>(e => e.InvoiceId == invoiceId).Count();
            if(count>0)
            {
                throw new InvalidOperationException("The specified invoice has already been associated to a Job Order.");
            }
            var @event = new IncomingInvoiceAssociatedToJobOrderEvent(invoiceId, this.Id);
            RaiseEvent(@event);
        }

        /// <summary>
        /// Associate an outgoing invoice to the current Job Order
        /// </summary>
        /// <param name="eventStore">The event store</param>
        /// <param name="invoiceId">The Id of the Invoice to be associated to the current Job Order</param>
        /// <exception cref="InvalidOperationException">Thrown if the specified invoiceId refers to an invoice which has already been associated to a Job Order</exception>
        public void AssociateOutgoingInvoice(IEventStore eventStore, Guid invoiceId)
        {
            var count = eventStore.Find<OutgoingInvoiceAssociatedToJobOrderEvent>(e => e.InvoiceId == invoiceId).Count();
            if (count > 0)
            {
                throw new InvalidOperationException("The specified invoice has already been associated to a Job Order.");
            }
            var @event = new OutgoingInvoiceAssociatedToJobOrderEvent(invoiceId, this.Id);
            RaiseEvent(@event);
        }

        public virtual decimal CalculateBalance(IEventStore es)
        {
            return CalculateBalance(es, DateTime.Now);
        }

        public virtual decimal CalculateBalance(IEventStore es, DateTime balanceDate)
        {
            if (es == null)
            {
                throw new ArgumentNullException("es");
            }
            var outgoingInvoicesIds = es.Find<OutgoingInvoiceAssociatedToJobOrderEvent>(e => e.JobOrderId == this.Id && e.TimeStamp <= balanceDate).Select(e => e.InvoiceId);
            var earnings = es.Find<OutgoingInvoiceIssuedEvent>(e => outgoingInvoicesIds.Contains(e.InvoiceId)).Sum(e => e.Amount);

            var incomingInvoicesIds = es.Find<IncomingInvoiceAssociatedToJobOrderEvent>(e => e.JobOrderId == this.Id && e.TimeStamp <= balanceDate).Select(e => e.InvoiceId);
            var costs = es.Find<IncomingInvoiceRegisteredEvent>(e => outgoingInvoicesIds.Contains(e.InvoiceId)).Sum(e => e.Amount);
            
            decimal balance = earnings - costs;

            return balance;
        }
    }
}
