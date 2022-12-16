using Merp.Accountancy.CommandStack.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merp.Accountancy.CommandStack.Model
{
    public class IncomingInvoice : Invoice
    {
        protected IncomingInvoice()
        {

        }

        public static class Factory
        {
            public static IncomingInvoice Issue(string invoiceNumber, DateTime invoiceDate, decimal amount, decimal taxes, decimal totalPrice, string description, string paymentTerms, string purchaseOrderNumber, Guid customerId, string customerName)
            {
                var invoice = new IncomingInvoice()
                {
                    Id = Guid.NewGuid(),
                    Number = invoiceNumber,
                    Date = invoiceDate,
                    Amount = amount,
                    Taxes = taxes,
                    TotalPrice = totalPrice,
                    Description = description,
                    PaymentTerms = paymentTerms,
                    PurchaseOrderNumber = purchaseOrderNumber,
                    Customer = new PartyInfo(customerId, customerName, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty)
                };
                var @event = new IncomingInvoiceRegisteredEvent(
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
