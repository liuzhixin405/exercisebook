using Merp.Accountancy.CommandStack.Commands;
using Merp.Accountancy.QueryStack;
using Merp.Infrastructure;
using Merp.Web.UI.Areas.Accountancy.Models.Invoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Merp.Web.UI.Areas.Accountancy.WorkerServices
{
    public class InvoiceControllerWorkerServices
    {
        public IBus Bus { get; private set; }
        public IDatabase Database { get; set; }

        public InvoiceControllerWorkerServices(IBus bus, IDatabase database)
        {
            if(bus==null)
            {
                throw new ArgumentNullException("bus");
            }
            if (database == null)
            {
                throw new ArgumentNullException("database");
            }
            this.Bus = bus;
            this.Database = database;
        }
        public IssueViewModel GetIssueViewModel()
        {
            var model = new IssueViewModel();
            model.Date = DateTime.Now;
            return model;
        }
        public RegisterViewModel GetRegisterViewModel()
        {
            var model = new RegisterViewModel();
            model.Date = DateTime.Now;
            return model;
        }
        public void Issue(IssueViewModel model)
        {
            var command = new IssueInvoiceCommand(
                model.Date,
                model.Amount,
                model.Taxes,
                model.TotalPrice,
                model.Description,
                model.PaymentTerms,
                model.PurchaseOrderNumber,
                model.Customer.OriginalId,
                model.Customer.Name,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty
                );
            Bus.Send(command);
        }
        public void Register(RegisterViewModel model)
        {
            var command = new RegisterIncomingInvoiceCommand(
                model.InvoiceNumber,
                model.Date,
                model.Amount,
                model.Taxes,
                model.TotalPrice,
                model.Description,
                model.PaymentTerms,
                model.PurchaseOrderNumber,
                model.Supplier.OriginalId,
                model.Supplier.Name,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty
                );
            Bus.Send(command);
        }

        #region AssignIncomingInvoiceToJobOrder
        public IEnumerable<IncomingInvoicesNotAssignedToAJobOrderViewModel.Invoice> GetListOfIncomingInvoicesNotAssignedToAJobOrderViewModel()
        {
            var model = (from i in Database.IncomingInvoices.NotAssociatedToAnyJobOrder()
                        orderby i.Date
                        select new IncomingInvoicesNotAssignedToAJobOrderViewModel.Invoice
                        {
                            Amount = i.Amount,
                            Number = i.Number,
                            SupplierName = i.Supplier.Name,
                            OriginalId = i.OriginalId
                        }).ToArray();
            return model;
        }

        public AssignIncomingInvoiceToJobOrderViewModel GetAssignIncomingInvoiceToJobOrderViewModel(Guid invoiceId)
        {
            var model = (from i in Database.IncomingInvoices
                        where i.OriginalId == invoiceId
                        select new AssignIncomingInvoiceToJobOrderViewModel
                        {
                            Amount = i.Amount,
                            Date = i.Date,
                            InvoiceNumber = i.Number,
                            InvoiceOriginalId = i.OriginalId,
                            SupplierName = i.Supplier.Name
                        }).Single();
            return model;
        }

        public void AssignIncomingInvoiceToJobOrder(AssignIncomingInvoiceToJobOrderViewModel model, string jobOrderNumber)
        {
            var jobOrderId = (from j in Database.JobOrders
                                 where j.Number == jobOrderNumber
                                 select j.OriginalId).Single();
            var command = new AssociateIncomingInvoiceToJobOrderCommand(model.InvoiceOriginalId, jobOrderId);
            Bus.Send(command);
        }
        #endregion

        #region AssignOutgoingInvoiceToJobOrder
        public IEnumerable<OutgoingInvoicesNotAssignedToAJobOrderViewModel.Invoice> GetListOfOutgoingInvoicesNotAssignedToAJobOrderViewModel()
        {
            var model = (from i in Database.OutgoingInvoices.NotAssociatedToAnyJobOrder()
                         orderby i.Date
                         select new OutgoingInvoicesNotAssignedToAJobOrderViewModel.Invoice
                         {
                             Amount = i.Amount,
                             Number = i.Number,
                             CustomerName = i.Customer.Name,
                             OriginalId = i.OriginalId
                         }).ToArray();
            return model;
        }

        public AssignOutgoingInvoiceToJobOrderViewModel GetAssignOutgoingInvoiceToJobOrderViewModel(Guid invoiceId)
        {
            var model = (from i in Database.OutgoingInvoices
                         where i.OriginalId == invoiceId
                         select new AssignOutgoingInvoiceToJobOrderViewModel
                         {
                             Amount = i.Amount,
                             Date = i.Date,
                             InvoiceNumber = i.Number,
                             InvoiceOriginalId = i.OriginalId,
                             CustomerName = i.Customer.Name
                         }).Single();
            return model;
        }

        public void AssignOutgoingInvoiceToJobOrder(AssignOutgoingInvoiceToJobOrderViewModel model, string jobOrderNumber)
        {
            var jobOrderId = (from j in Database.JobOrders
                              where j.Number == jobOrderNumber
                              select j.OriginalId).Single();
            var command = new AssociateOutgoingInvoiceToJobOrderCommand(model.InvoiceOriginalId, jobOrderId);
            Bus.Send(command);
        }
        #endregion
    }
}