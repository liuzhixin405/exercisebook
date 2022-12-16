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
    public class OutgoingInvoice : Invoice
    {
        protected OutgoingInvoice()
        {

        }

        public static class Factory
        {
            public static OutgoingInvoice Issue(IOutgoingInvoiceNumberGenerator generator, DateTime invoiceDate, decimal amount, decimal taxes, decimal totalPrice, string description, string paymentTerms, string purchaseOrderNumber, Guid customerId, string customerName)
            {
                var invoice = new OutgoingInvoice()
                {
                    Id = Guid.NewGuid(),
                    Number = generator.Generate(),
                    Date = invoiceDate,
                    Amount=amount,
                    Taxes=taxes,
                    TotalPrice=totalPrice,
                    Description=description,
                    PaymentTerms = paymentTerms,
                    PurchaseOrderNumber = purchaseOrderNumber,
                    Customer = new PartyInfo(customerId, customerName, string.Empty, string.Empty, string.Empty,string.Empty, string.Empty, string.Empty)
                };
                var @event = new OutgoingInvoiceIssuedEvent(
                    invoice.Id,
                    invoice.Number,
                    invoice.Date,
                    invoice.Amount,
                    invoice.Taxes,
                    invoice.TotalPrice,
                    invoice.Description,
                    invoice.PaymentTerms,
                    invoice.PurchaseOrderNumber,
                    invoice.Customer.Id,
                    invoice.Customer.Name,
                    invoice.Customer.StreetName,
                    invoice.Customer.City,
                    invoice.Customer.PostalCode,
                    invoice.Customer.Country,
                    invoice.Customer.VatIndex,
                    invoice.Customer.NationalIdentificationNumber
                    );
                invoice.RaiseEvent(@event);
                return invoice;
            }
        }
    }
}
