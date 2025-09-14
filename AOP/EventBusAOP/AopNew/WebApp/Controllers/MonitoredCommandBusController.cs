using Common.Bus.Core;
using Common.Bus.Implementations;
using Common.Bus.Monitoring;
using Microsoft.AspNetCore.Mvc;
using WebApp.Commands;

namespace WebApp.Controllers
{
    /// <summary>
    /// 带监控的CommandBus演示控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class MonitoredCommandBusController : ControllerBase
    {
        private readonly MonitoredCommandBus _commandBus;
        private readonly ILogger<MonitoredCommandBusController> _logger;

        public MonitoredCommandBusController(
            MonitoredCommandBus commandBus,
            ILogger<MonitoredCommandBusController> logger)
        {
            _commandBus = commandBus;
            _logger = logger;
        }

        /// <summary>
        /// 处理订单 - 使用带监控的CommandBus
        /// </summary>
        [HttpPost("process-order")]
        public async Task<IActionResult> ProcessOrder([FromBody] ProcessOrderCommand command)
        {
            try
            {
                var result = await _commandBus.SendAsync<ProcessOrderCommand, string>(command);
                
                return Ok(new { Success = true, Result = result, BusType = "Monitored" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing order with Monitored CommandBus");
                return BadRequest(new { Success = false, Error = ex.Message });
            }
        }

        /// <summary>
        /// 创建用户 - 使用带监控的CommandBus
        /// </summary>
        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
        {
            try
            {
                var result = await _commandBus.SendAsync<CreateUserCommand, int>(command);
                
                return Ok(new { Success = true, UserId = result, BusType = "Monitored" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user with Monitored CommandBus");
                return BadRequest(new { Success = false, Error = ex.Message });
            }
        }

        /// <summary>
        /// 发送邮件 - 使用带监控的CommandBus
        /// </summary>
        [HttpPost("send-email")]
        public async Task<IActionResult> SendEmail([FromBody] SendEmailCommand command)
        {
            try
            {
                var result = await _commandBus.SendAsync<SendEmailCommand, bool>(command);
                
                return Ok(new { Success = true, EmailSent = result, BusType = "Monitored" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email with Monitored CommandBus");
                return BadRequest(new { Success = false, Error = ex.Message });
            }
        }

        /// <summary>
        /// 并发处理订单 - 使用带监控的CommandBus
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
                    BusType = "Monitored"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error concurrent processing orders with Monitored CommandBus");
                return BadRequest(new { Success = false, Error = ex.Message });
            }
        }

        /// <summary>
        /// 获取带监控的CommandBus指标
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
                        BusType = "Monitored"
                    });
                }
                
                return Ok(new { 
                    Success = true, 
                    Message = "Metrics not available for this CommandBus type",
                    BusType = "Monitored"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Monitored CommandBus metrics");
                return BadRequest(new { Success = false, Error = ex.Message });
            }
        }
    }
}
