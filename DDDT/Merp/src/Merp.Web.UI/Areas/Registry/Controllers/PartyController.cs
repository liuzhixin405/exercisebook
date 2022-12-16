using Merp.Web.UI.Areas.Registry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcMate.Web.Mvc;
using Merp.Web.UI.Areas.Registry.WorkerServices;

namespace Merp.Web.UI.Areas.Registry.Controllers
{
    public class PartyController : Controller
    {
        public PartyControllerWorkerServices WorkerServices { get; private set; }

        public PartyController(PartyControllerWorkerServices workerServices)
        {
            if(workerServices==null)
            {
                throw new ArgumentNullException("workerServices");
            }
            WorkerServices = workerServices;
        }

        [HttpGet]
        public ActionResult Detail(int id)
        {
            switch (WorkerServices.GetDetailViewModel(id))
            {
                case "Company":
                    return Redirect(string.Format("/Registry/Company/Detail/{0}", id));
                case "Person":
                    return Redirect(string.Format("/Registry/Person/Detail/{0}", id));
                default:
                    return RedirectToAction("Search");
            }
        }

        [HttpGet]
        public ActionResult Search()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetParties(string query)
        {
            var model = WorkerServices.GetParties(query);
            return this.Jsonp(model);
        }

        [HttpGet]
        public ActionResult GetPartyInfoByPattern(string text)
        {
            var model = WorkerServices.GetPartyNamesByPattern(text);
            return this.Jsonp(model);
        }

        [HttpGet]
        public ActionResult GetPartyInfoById(int id)
        {
            var model = WorkerServices.GetPartyInfoByPattern(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetPersonInfoByPattern(string text)
        {
            var model = WorkerServices.GetPersonNamesByPattern(text);
            return this.Jsonp(model);
        }

        [HttpGet]
        public ActionResult GetPersonInfoById(int id)
        {
            var model = WorkerServices.GetPersonInfoByPattern(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}