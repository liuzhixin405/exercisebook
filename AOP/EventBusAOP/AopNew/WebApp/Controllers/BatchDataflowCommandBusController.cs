using Common.Bus.Core;
using Common.Bus.Implementations;
using Common.Bus.Monitoring;
using Microsoft.AspNetCore.Mvc;
using WebApp.Commands;

namespace WebApp.Controllers
{
    /// <summary>
    /// 批处理Dataflow CommandBus演示控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BatchDataflowCommandBusController : ControllerBase
    {
        private readonly BatchDataflowCommandBus _commandBus;
        private readonly ILogger<BatchDataflowCommandBusController> _logger;

        public BatchDataflowCommandBusController(
            BatchDataflowCommandBus commandBus,
            ILogger<BatchDataflowCommandBusController> logger)
        {
            _commandBus = commandBus;
            _logger = logger;
        }

        /// <summary>
        /// 处理订单 - 使用批处理Dataflow CommandBus
        /// </summary>
        [HttpPost("process-order")]
        public async Task<IActionResult> ProcessOrder([FromBody] ProcessOrderCommand command)
        {
            try
            {
                
                var result = await _commandBus.SendAsync<ProcessOrderCommand, string>(command);
                
                return Ok(new { Success = true, Result = result, BusType = "BatchDataflow" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing order with BatchDataflow CommandBus");
                return BadRequest(new { Success = false, Error = ex.Message });
            }
        }

        /// <summary>
        /// 创建用户 - 使用批处理Dataflow CommandBus
        /// </summary>
        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
        {
            try
            {
                
                var result = await _commandBus.SendAsync<CreateUserCommand, int>(command);
                
                return Ok(new { Success = true, UserId = result, BusType = "BatchDataflow" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user with BatchDataflow CommandBus");
                return BadRequest(new { Success = false, Error = ex.Message });
            }
        }

        /// <summary>
        /// 发送邮件 - 使用批处理Dataflow CommandBus
        /// </summary>
        [HttpPost("send-email")]
        public async Task<IActionResult> SendEmail([FromBody] SendEmailCommand command)
        {
            try
            {
                
                var result = await _commandBus.SendAsync<SendEmailCommand, bool>(command);
                
                return Ok(new { Success = true, EmailSent = result, BusType = "BatchDataflow" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email with BatchDataflow CommandBus");
                return BadRequest(new { Success = false, Error = ex.Message });
            }
        }

        /// <summary>
        /// 批量处理订单 - 使用批处理Dataflow CommandBus
        /// </summary>
        [HttpPost("batch-process-orders")]
        public async Task<IActionResult> BatchProcessOrders([FromBody] List<ProcessOrderCommand> commands)
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
                    BusType = "BatchDataflow",
                    BatchSize = 5,
                    BatchTimeout = "200ms"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error batch processing orders with BatchDataflow CommandBus");
                return BadRequest(new { Success = false, Error = ex.Message });
            }
        }

        /// <summary>
        /// 获取批处理Dataflow CommandBus指标
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
                        BusType = "BatchDataflow"
                    });
                }
                
                return Ok(new { 
                    Success = true, 
                    Message = "Metrics not available for this CommandBus type",
                    BusType = "BatchDataflow"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting BatchDataflow CommandBus metrics");
                return BadRequest(new { Success = false, Error = ex.Message });
            }
        }
    }
}
