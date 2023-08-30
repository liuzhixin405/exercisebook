using Merp.Web.UI.Areas.Registry.Models.Person;
using Merp.Web.UI.Areas.Registry.WorkerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Merp.Web.UI.Areas.Registry.Controllers
{
    public class PersonController : Controller
    {
        public PersonControllerWorkerServices WorkerServices { get; private set; }

        public PersonController(PersonControllerWorkerServices workerServices)
        {
            if(workerServices==null)
            {
                throw new ArgumentNullException("workerServices");
            }
            WorkerServices = workerServices;
        }

        [HttpGet]
        public ActionResult AddEntry()
        {
            var model = new AddEntryViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult AddEntry(AddEntryViewModel model)
        {
            if(!this.ModelState.IsValid)
            {
                return View(model);
            }
            WorkerServices.AddEntry(model);
            return Redirect("/Registry/");
        }
    }
}