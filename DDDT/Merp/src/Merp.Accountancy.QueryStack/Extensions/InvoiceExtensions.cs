using Merp.Accountancy.QueryStack.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merp.Accountancy.QueryStack
{
    public static class InvoiceExtensions
    {
        private static IQueryable<T> _NotAssociatedToAnyJobOrder<T>(this IQueryable<T> invoices) where T : Invoice
        {
            return invoices.Where(i => !i.JobOrderId.HasValue);
        }

        public static IQueryable<IncomingInvoice> NotAssociatedToAnyJobOrder(this IQueryable<IncomingInvoice> invoices)
        {
            return _NotAssociatedToAnyJobOrder(invoices);
        }

        public static IQueryable<OutgoingInvoice> NotAssociatedToAnyJobOrder(this IQueryable<OutgoingInvoice> invoices)
        {
            return _NotAssociatedToAnyJobOrder(invoices);
        }
    }
}
