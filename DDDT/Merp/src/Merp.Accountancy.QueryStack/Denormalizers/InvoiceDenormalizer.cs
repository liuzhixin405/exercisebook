using Merp.Accountancy.CommandStack.Events;
using Merp.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merp.Accountancy.QueryStack.Denormalizers
{
    public class InvoiceDenormalizer :
        IHandleMessage<IncomingInvoiceAssociatedToJobOrderEvent>,
        IHandleMessage<OutgoingInvoiceAssociatedToJobOrderEvent>
    {
        public void Handle(IncomingInvoiceAssociatedToJobOrderEvent message)
        {
            using(var ctx = new AccountancyContext())
            {
                var invoice = ctx.IncomingInvoices.Where(i => i.OriginalId == message.InvoiceId).Single();
                invoice.JobOrderId = message.JobOrderId;
                ctx.SaveChanges();
            }
        }

        public void Handle(OutgoingInvoiceAssociatedToJobOrderEvent message)
        {
            using (var ctx = new AccountancyContext())
            {
                var invoice = ctx.OutgoingInvoices.Where(i => i.OriginalId == message.InvoiceId).Single();
                invoice.JobOrderId = message.JobOrderId;
                ctx.SaveChanges();
            }
        }
    }
}
