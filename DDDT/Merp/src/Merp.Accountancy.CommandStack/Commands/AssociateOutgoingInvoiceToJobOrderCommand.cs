using Merp.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merp.Accountancy.CommandStack.Commands
{
    public class AssociateOutgoingInvoiceToJobOrderCommand : Command
    {
        public Guid JobOrderId { get; private set; }
        public Guid InvoiceId { get; private set; }

        public AssociateOutgoingInvoiceToJobOrderCommand(Guid invoiceId, Guid jobOrderId)
        {
            InvoiceId = invoiceId;
            JobOrderId = jobOrderId;
        }
    }
}
