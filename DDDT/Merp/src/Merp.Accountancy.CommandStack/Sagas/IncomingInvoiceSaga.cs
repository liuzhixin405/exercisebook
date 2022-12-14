using Merp.Accountancy.CommandStack.Commands;
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
    public class IncomingInvoiceSaga : Saga,
        IAmStartedBy<RegisterIncomingInvoiceCommand>
    {

        public IncomingInvoiceSaga(IBus bus, IEventStore eventStore, IRepository repository)
            : base(bus, eventStore, repository)
        {
        }

        public void Handle(RegisterIncomingInvoiceCommand message)
        {
            var invoice = IncomingInvoice.Factory.Issue(
                message.InvoiceNumber,
                message.InvoiceDate,
                message.Amount,
                message.Taxes,
                message.TotalPrice,
                message.Description,
                message.PaymentTerms,
                message.PurchaseOrderNumber,
                message.Customer.Id,
                message.Customer.Name
                );
            this.Repository.Save(invoice);
        }
    }
}
