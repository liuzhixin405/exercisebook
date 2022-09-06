using Microsoft.AspNetCore.Mvc;
using SerilogTimings;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreSerilogDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SeriLogDemoController : ControllerBase
    {


        private readonly ILogger<SeriLogDemoController> _logger;
        private readonly Serilog.ILogger _seriLogger;
        public SeriLogDemoController(ILogger<SeriLogDemoController> logger, Serilog.ILogger serilogLogger)
        {
            _logger = logger;
            _seriLogger = serilogLogger;
        }

        [HttpGet]
        public string String()
        {
            _logger.LogInformation("this is serilog...");
            return "Suscess";
        }

        [HttpGet("customers/{id}")]
        public string CreateCusomer(int id)
        {
            _logger.LogInformation("Creating customer {id}",id);  //可以通过id = 112查询到跟踪信息112给的参数
            //_logger.LogInformation($"Creating customer {id}");   //跟踪不到id
            return "Customer";
        }

        [HttpGet("simple")]
        public IActionResult Get()
        {
            // Log messages with structured content embedded into msg text.
            // The following line uses the ASP.NET core logging abstraction.
            _logger.LogInformation("Entered {Controller}.{Method}", nameof(SeriLogDemoController), nameof(Get));

            // The following line demonstrates how we could use serilog's
            // own abstraction. Offers more features than ASP.NET core logging.
            _seriLogger
                .ForContext("Controller", nameof(SeriLogDemoController))
                .ForContext("Method", nameof(Get))
                .Warning("Entered");

            return Ok();
        }

        [HttpGet("exception")]
        public IActionResult HandledException()
        {
            try
            {
                throw new InvalidOperationException("Something bad happened");
            }
            catch (InvalidOperationException ex)
            {
                // Log errors including exception details
                _logger.LogError(ex, "Error in {Method}", nameof(HandledException));
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

#pragma warning disable CS0162 // Unreachable code detected
            return Ok();
#pragma warning restore CS0162 // Unreachable code detected
        }

        [HttpGet("timing")]
        public async Task<IActionResult> Timings()
        {
            // You can log timing of critical operations (e.g. DB queries)
            // using the "serilog timings" NuGet package. Read more at
            // https://github.com/nblumhardt/serilog-timings.
            using (Operation.Time("Some long running operation for {OrderId}", 42))
            {
                await Task.Delay(500);
            }

            return Ok();
        }

        [HttpGet("unhandled-exception")]
        public IActionResult UnhandledException() => throw new InvalidOperationException("Something bad happened");


        [HttpPost("customers")]
        public IActionResult CreateCustomer(CustomerDto customer)
        {
            // Note that validation of customer is done by ASP.NET Core automatically.

            // Simulate adding to DB
            _logger.LogInformation("Writing customer {CustomerName} to DB", customer.Name);

            return StatusCode(StatusCodes.Status201Created);
        }
      
    }
    public record CustomerDto([Required][MaxLength(50)] string Name, [Range(0, 100)] int Age);
    public class CustomerDtoClass
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Range(0, 100)]
        public int Age { get; set; }
    }
}