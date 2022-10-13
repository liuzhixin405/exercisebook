using LiveScore_ES.Services.Home;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LiveScore_ES.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly HomeService _homeService;
        public HomeController(HomeService homeService)
        {
            _homeService = homeService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name">Start、End、NewPeriod、EndPeriod、Goal1、Goal2、Undo、Zap</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Action(string id,string name)
        {
            _homeService.DispatchCommand(id, name.ToLower());
            return Ok();
        }

        [HttpGet]
        public IActionResult GetCurrentState(string id)
        {
            var model = _homeService.GetCurrentState(id);
            return Ok(model);
        }
    }
}
