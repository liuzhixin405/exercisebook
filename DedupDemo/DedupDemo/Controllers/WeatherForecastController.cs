using Microsoft.AspNetCore.Mvc;
using MinHashSharp;

namespace DedupDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly MinHashLSH _minHashLSH;
      

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, MinHashLSH minHashLSH)
        {
            _logger = logger;
            _minHashLSH = minHashLSH;
        }

        [HttpPost]
        public IActionResult Deduplicate([FromBody] string text)
        {
            string s1 = "The quick brown fox jumps over the lazy dog and proceeded to run towards the other room";
            string s2 = "The slow purple elephant runs towards the happy fox and proceeded to run towards the other room";
            string s3 = "The quick brown fox jumps over the angry dog and proceeded to run towards the other room";

            var m1 = new MinHash(numPerm: 128).Update(s1.Split());
            var m2 = new MinHash(numPerm: 128).Update(s2.Split());
            var m3 = new MinHash(numPerm: 128).Update(s3.Split());

            _minHashLSH.Insert("s1", m1);
            _minHashLSH.Insert("s2", m2);
            return Ok(string.Join(", ", _minHashLSH.Query(m3)));
        }
    }
}

