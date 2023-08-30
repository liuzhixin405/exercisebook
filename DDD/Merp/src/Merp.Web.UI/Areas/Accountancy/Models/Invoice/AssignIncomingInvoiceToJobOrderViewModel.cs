using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Merp.Web.UI.Areas.Accountancy.Models.Invoice
{
    public class AssignIncomingInvoiceToJobOrderViewModel
    {
        public string InvoiceNumber { get; set; }
        public Guid InvoiceOriginalId { get; set; }
        public string SupplierName { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        [Required]
        public string JobOrderNumber { get; set; }
    }
}