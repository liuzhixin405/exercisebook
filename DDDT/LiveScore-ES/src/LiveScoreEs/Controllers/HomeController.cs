using System;
using System.Web;
using System.Web.Mvc;
using LiveScoreEs.Services.Home;

namespace LiveScoreEs.Controllers
{
    public class HomeController : Controller
    {
        private readonly HomeService _service;

        public HomeController() : this (new HomeService()) 
        {
        }
        public HomeController(HomeService service)
        {
            _service = service;
        }

        public ActionResult Index(String id)
        {
            id = id ?? "WP001";
            var model = _service.GetCurrentState(id);
            return View(model);
        }

        public ActionResult Action(String id)
        {
            var eventName = MakeSenseOfWhatTheUserDid(Request);

            // Perform action here
            _service.DispatchCommand(id, eventName.ToLower());

            return RedirectToAction("index", new {id = id});
        }

        private String MakeSenseOfWhatTheUserDid(HttpRequestBase request)
        {
            String buffer;
            buffer = request.Params["btnStart"];
            if (buffer != null) return "Start";

            buffer = request.Params["btnEnd"];
            if (buffer != null) return "End";

            buffer = request.Params["btnNewPeriod"];
            if (buffer != null) return "NewPeriod";

            buffer = request.Params["btnEndPeriod"];
            if (buffer != null) return "EndPeriod";

            buffer = request.Params["btnGoal1"];
            if (buffer != null) return "Goal1";

            buffer = request.Params["btnGoal2"];
            if (buffer != null) return "Goal2";

            buffer = request.Params["btnUndo"];
            if (buffer != null) return "Undo";

            buffer = request.Params["btnZap"];
            if (buffer != null) return "Zap";

            return buffer;
        }
	}
}