using Common.Bus.Core;
using Common.Bus.Implementations;
using Microsoft.AspNetCore.Mvc;
using WebApp.Commands;

namespace WebApp.Controllers
{
    /// <summary>
    /// 标准CommandBus演示控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class StandardCommandBusController : ControllerBase
    {
        private readonly CommandBus _commandBus;
        private readonly ILogger<StandardCommandBusController> _logger;

        public StandardCommandBusController(
            CommandBus commandBus,
            ILogger<StandardCommandBusController> logger)
        {
            _commandBus = commandBus;
            _logger = logger;
        }

        /// <summary>
        /// 处理订单 - 使用标准CommandBus
        /// </summary>
        [HttpPost("process-order")]
        public async Task<IActionResult> ProcessOrder([FromBody] ProcessOrderCommand command)
        {
            try
            {
                var result = await _commandBus.SendAsync<ProcessOrderCommand, string>(command);
                
                return Ok(new { Success = true, Result = result, BusType = "Standard" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing order with Standard CommandBus");
                return BadRequest(new { Success = false, Error = ex.Message });
            }
        }

        /// <summary>
        /// 创建用户 - 使用标准CommandBus
        /// </summary>
        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
        {
            try
            {
                var result = await _commandBus.SendAsync<CreateUserCommand, int>(command);
                
                return Ok(new { Success = true, UserId = result, BusType = "Standard" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user with Standard CommandBus");
                return BadRequest(new { Success = false, Error = ex.Message });
            }
        }

        /// <summary>
        /// 发送邮件 - 使用标准CommandBus
        /// </summary>
        [HttpPost("send-email")]
        public async Task<IActionResult> SendEmail([FromBody] SendEmailCommand command)
        {
            try
            {
                var result = await _commandBus.SendAsync<SendEmailCommand, bool>(command);
                
                return Ok(new { Success = true, EmailSent = result, BusType = "Standard" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email with Standard CommandBus");
                return BadRequest(new { Success = false, Error = ex.Message });
            }
        }

        /// <summary>
        /// 批量处理订单 - 使用标准CommandBus
        /// </summary>
        [HttpPost("batch-process-orders")]
        public async Task<IActionResult> BatchProcessOrders([FromBody] List<ProcessOrderCommand> commands)
        {
            try
            {
                var results = new List<string>();
                
                foreach (var command in commands)
                {
                    var result = await _commandBus.SendAsync<ProcessOrderCommand, string>(command);
                    results.Add(result);
                }
                
                return Ok(new { 
                    Success = true, 
                    Results = results, 
                    Count = results.Count,
                    BusType = "Standard" 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error batch processing orders with Standard CommandBus");
                return BadRequest(new { Success = false, Error = ex.Message });
            }
        }
    }
}
