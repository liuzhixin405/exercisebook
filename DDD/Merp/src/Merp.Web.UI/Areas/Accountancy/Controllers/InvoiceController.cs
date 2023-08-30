using Merp.Web.UI.Areas.Accountancy.Models.Invoice;
using Merp.Web.UI.Areas.Accountancy.WorkerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcMate.Web.Mvc;

namespace Merp.Web.UI.Areas.Accountancy.Controllers
{
    public class InvoiceController : Controller
    {
        public InvoiceControllerWorkerServices WorkerServices { get; private set; }

        public InvoiceController(InvoiceControllerWorkerServices workerServices)
        {
            if(workerServices==null)
            {
                throw new ArgumentNullException("workerServices");
            }
            WorkerServices = workerServices;
        }

        [HttpGet]
        public ActionResult Issue()
        {
            var model = WorkerServices.GetIssueViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Issue(IssueViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return View(model);
            }
            WorkerServices.Issue(model);
            return Redirect("/Accountancy/");
        }

        [HttpGet]
        public ActionResult Register()
        {
            var model = WorkerServices.GetRegisterViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return View(model);
            }
            WorkerServices.Register(model);
            return Redirect("/Accountancy/");
        }

        #region AssignIncomingInvoiceToJobOrder
        [HttpGet]
        public ActionResult ListOfIncomingInvoicesNotAssignedToAJobOrder()
        {
            var model = WorkerServices.GetListOfIncomingInvoicesNotAssignedToAJobOrderViewModel();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult IncomingInvoicesNotAssignedToAJobOrder()
        {
            var model = new IncomingInvoicesNotAssignedToAJobOrderViewModel();
            return View(model);
        }

        [HttpGet]
        public ActionResult AssignIncomingInvoiceToJobOrder(Guid? id)
        {
            var model = WorkerServices.GetAssignIncomingInvoiceToJobOrderViewModel(id.Value);
            return View(model);
        }

        [HttpPost]
        public ActionResult AssignIncomingInvoiceToJobOrder(AssignIncomingInvoiceToJobOrderViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return View(model);
            }
            WorkerServices.AssignIncomingInvoiceToJobOrder(model, model.JobOrderNumber);
            return Redirect("/Accountancy/");
        }
        #endregion

        #region AssignOutgoingInvoiceToJobOrder
        [HttpGet]
        public ActionResult ListOfOutgoingInvoicesNotAssignedToAJobOrder()
        {
            var model = WorkerServices.GetListOfOutgoingInvoicesNotAssignedToAJobOrderViewModel();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult OutgoingInvoicesNotAssignedToAJobOrder()
        {
            var model = new OutgoingInvoicesNotAssignedToAJobOrderViewModel();
            return View(model);
        }

        [HttpGet]
        public ActionResult AssignOutgoingInvoiceToJobOrder(Guid? id)
        {
            var model = WorkerServices.GetAssignOutgoingInvoiceToJobOrderViewModel(id.Value);
            return View(model);
        }

        [HttpPost]
        public ActionResult AssignOutgoingInvoiceToJobOrder(AssignOutgoingInvoiceToJobOrderViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return View(model);
            }
            WorkerServices.AssignOutgoingInvoiceToJobOrder(model, model.JobOrderNumber);
            return Redirect("/Accountancy/");
        }
        #endregion
    }
}