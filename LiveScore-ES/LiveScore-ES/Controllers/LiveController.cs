using LiveScore_ES.Backend.ReadModel.Dto;
using LiveScore_ES.Services.Live;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LiveScore_ES.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LiveController : ControllerBase
    {
        private readonly LiveService _liveService;
        public LiveController(LiveService liveService)
        {
            _liveService = liveService;
        }

        [HttpGet]
        public IActionResult Index([Required]string id)
        {
            var model = _liveService.GetLiveMatch(id);
            return Ok(model);
        }
    }
}
