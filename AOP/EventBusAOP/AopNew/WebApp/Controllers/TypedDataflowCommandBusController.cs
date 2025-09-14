using Common.Bus.Core;
using Common.Bus.Implementations;
using Common.Bus.Monitoring;
using Microsoft.AspNetCore.Mvc;
using WebApp.Commands;

namespace WebApp.Controllers
{
    /// <summary>
    /// 类型安全Dataflow CommandBus演示控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TypedDataflowCommandBusController : ControllerBase
    {
        private readonly TypedDataflowCommandBus _commandBus;
        private readonly ILogger<TypedDataflowCommandBusController> _logger;

        public TypedDataflowCommandBusController(
            TypedDataflowCommandBus commandBus,
            ILogger<TypedDataflowCommandBusController> logger)
        {
            _commandBus = commandBus;
            _logger = logger;
        }

        /// <summary>
        /// 处理订单 - 使用类型安全Dataflow CommandBus
        /// </summary>
        [HttpPost("process-order")]
        public async Task<IActionResult> ProcessOrder([FromBody] ProcessOrderCommand command)
        {
            try
            {
                var result = await _commandBus.SendAsync<ProcessOrderCommand, string>(command);
                
                return Ok(new { Success = true, Result = result, BusType = "TypedDataflow" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing order with TypedDataflow CommandBus");
                return BadRequest(new { Success = false, Error = ex.Message });
            }
        }

        /// <summary>
        /// 创建用户 - 使用类型安全Dataflow CommandBus
        /// </summary>
        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
        {
            try
            {
                var result = await _commandBus.SendAsync<CreateUserCommand, int>(command);
                
                return Ok(new { Success = true, UserId = result, BusType = "TypedDataflow" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user with TypedDataflow CommandBus");
                return BadRequest(new { Success = false, Error = ex.Message });
            }
        }

        /// <summary>
        /// 发送邮件 - 使用类型安全Dataflow CommandBus
        /// </summary>
        [HttpPost("send-email")]
        public async Task<IActionResult> SendEmail([FromBody] SendEmailCommand command)
        {
            try
            {
                var result = await _commandBus.SendAsync<SendEmailCommand, bool>(command);
                
                return Ok(new { Success = true, EmailSent = result, BusType = "TypedDataflow" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email with TypedDataflow CommandBus");
                return BadRequest(new { Success = false, Error = ex.Message });
            }
        }

        /// <summary>
        /// 并发处理订单 - 使用类型安全Dataflow CommandBus
        /// </summary>
        [HttpPost("concurrent-process-orders")]
        public async Task<IActionResult> ConcurrentProcessOrders([FromBody] List<ProcessOrderCommand> commands)
        {
            try
            {
                var tasks = commands.Select(command => 
                    _commandBus.SendAsync<ProcessOrderCommand, string>(command));
                
                var results = await Task.WhenAll(tasks);
                
                return Ok(new { 
                    Success = true, 
                    Results = results, 
                    Count = results.Length,
                    BusType = "TypedDataflow",
                    Concurrency = 4
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error concurrent processing orders with TypedDataflow CommandBus");
                return BadRequest(new { Success = false, Error = ex.Message });
            }
        }

        /// <summary>
        /// 获取类型安全Dataflow CommandBus指标
        /// </summary>
        [HttpGet("metrics")]
        public IActionResult GetMetrics()
        {
            try
            {
                
                if (_commandBus is IMonitoredCommandBus monitoredBus)
                {
                    var metrics = monitoredBus.GetMetrics();
                    return Ok(new { 
                        Success = true, 
                        Metrics = metrics,
                        BusType = "TypedDataflow"
                    });
                }
                
                return Ok(new { 
                    Success = true, 
                    Message = "Metrics not available for this CommandBus type",
                    BusType = "TypedDataflow"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting TypedDataflow CommandBus metrics");
                return BadRequest(new { Success = false, Error = ex.Message });
            }
        }
    }
}
