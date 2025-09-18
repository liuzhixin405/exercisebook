using System;
using System.Threading.Tasks;
using Common.Bus.Core;
using Microsoft.AspNetCore.Mvc;
using WebApp.Commands;

namespace WebApp.Controllers
{
    /// <summary>
    /// 增强命令总线控制器 - 演示完整的AOP横切关注点
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class EnhancedCommandBusController : ControllerBase
    {
        private readonly EnhancedCommandBus _commandBus;

        public EnhancedCommandBusController(EnhancedCommandBus commandBus)
        {
            _commandBus = commandBus;
        }

        /// <summary>
        /// 处理订单 - 演示完整的AOP管道
        /// </summary>
        /// <param name="command">订单处理命令</param>
        /// <returns>处理结果</returns>
        [HttpPost("process-order")]
        public async Task<IActionResult> ProcessOrder([FromBody] ProcessOrderCommand command)
        {
            try
            {
                var result = await _commandBus.SendAsync<ProcessOrderCommand, string>(command);
                return Ok(new { message = "订单处理成功", result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "订单处理失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 创建用户 - 演示参数验证和返回值增强
        /// </summary>
        /// <param name="command">用户创建命令</param>
        /// <returns>创建结果</returns>
        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
        {
            try
            {
                var result = await _commandBus.SendAsync<CreateUserCommand, string>(command);
                return Ok(new { message = "用户创建成功", result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "用户创建失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 发送邮件 - 演示异常处理
        /// </summary>
        /// <param name="command">邮件发送命令</param>
        /// <returns>发送结果</returns>
        [HttpPost("send-email")]
        public async Task<IActionResult> SendEmail([FromBody] SendEmailCommand command)
        {
            try
            {
                var result = await _commandBus.SendAsync<SendEmailCommand, string>(command);
                return Ok(new { message = "邮件发送成功", result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "邮件发送失败", error = ex.Message });
            }
        }

        /// <summary>
        /// 获取增强命令总线的信息
        /// </summary>
        /// <returns>命令总线信息</returns>
        [HttpGet("info")]
        public IActionResult GetInfo()
        {
            return Ok(new
            {
                name = "Enhanced Command Bus",
                description = "支持完整AOP横切关注点的增强命令总线",
                features = new[]
                {
                    "参数贯穿处理 (Parameter Interception)",
                    "方法执行前处理 (Pre-Execution)",
                    "方法执行后处理 (Post-Execution)", 
                    "返回值贯穿处理 (Return Value Interception)",
                    "异常处理 (Exception Handling)",
                    "完整的日志记录",
                    "类型安全的泛型支持"
                },
                pipeline = new[]
                {
                    "1. 参数验证和转换",
                    "2. 执行前日志和权限检查",
                    "3. 命令处理器执行",
                    "4. 执行后日志和结果缓存",
                    "5. 返回值增强和格式化",
                    "6. 异常捕获和处理"
                }
            });
        }
    }
}
