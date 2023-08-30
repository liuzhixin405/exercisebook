using Merp.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merp.Accountancy.CommandStack.Events
{
    public class OutgoingInvoiceAssociatedToJobOrderEvent : DomainEvent
    {
        public Guid InvoiceId { get; private set; }
        public Guid JobOrderId { get; private set; }

        public OutgoingInvoiceAssociatedToJobOrderEvent(Guid invoiceId, Guid jobOrderId)
        {
            InvoiceId = invoiceId;
            JobOrderId = jobOrderId;
        }
    }
}
