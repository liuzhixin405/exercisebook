using Common.Bus;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace WebApp.Controllers
{
    /// <summary>
    /// 基于委托的泛型优化CommandBus演示控制器
    /// 展示如何通过委托简化泛型打包，保持类型安全
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DelegateBasedDemoController : ControllerBase
    {
        private readonly ICommandBus _commandBus;
        private readonly ILogger<DelegateBasedDemoController> _logger;

        public DelegateBasedDemoController(ICommandBus commandBus, ILogger<DelegateBasedDemoController> logger)
        {
            _commandBus = commandBus;
            _logger = logger;
        }

        /// <summary>
        /// 测试单个命令处理（委托优化版本）
        /// </summary>
        [HttpPost("single")]
        public async Task<IActionResult> ProcessSingleCommand([FromBody] DelegateBasedOrderCommand command)
        {
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                // 使用委托优化的CommandBus，保持泛型类型安全
                var result = await _commandBus.SendAsync<DelegateBasedOrderCommand, DelegateBasedOrderResult>(command);
                stopwatch.Stop();
                
                return Ok(new
                {
                    Result = result,
                    ProcessingTime = stopwatch.ElapsedMilliseconds,
                    RequestId = result.RequestId,
                    CreatedAt = result.CreatedAt,
                    Success = true,
                    OptimizationType = "Delegate-Based Generic Optimization"
                });
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Delegate-based command processing failed");
                
                return BadRequest(new
                {
                    Error = ex.Message,
                    ProcessingTime = stopwatch.ElapsedMilliseconds,
                    Success = false
                });
            }
        }

        /// <summary>
        /// 测试并发命令处理
        /// </summary>
        [HttpPost("concurrent")]
        public async Task<IActionResult> ProcessConcurrentCommands([FromBody] ConcurrentTestRequest request)
        {
            var stopwatch = Stopwatch.StartNew();
            var tasks = new List<Task<DelegateBasedOrderResult>>();
            
            _logger.LogInformation("Starting concurrent processing of {Count} commands", request.Count);
            
            // 创建并发任务
            for (int i = 0; i < request.Count; i++)
            {
                var command = new DelegateBasedOrderCommand($"Product-{i}", request.Quantity, request.Priority);
                tasks.Add(_commandBus.SendAsync<DelegateBasedOrderCommand, DelegateBasedOrderResult>(command));
            }
            
            try
            {
                var results = await Task.WhenAll(tasks);
                stopwatch.Stop();
                
                _logger.LogInformation("Completed concurrent processing in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
                
                return Ok(new
                {
                    Results = results,
                    TotalProcessingTime = stopwatch.ElapsedMilliseconds,
                    AverageProcessingTime = stopwatch.ElapsedMilliseconds / (double)request.Count,
                    Throughput = request.Count / stopwatch.Elapsed.TotalSeconds,
                    BatchSize = request.Count,
                    OptimizationType = "Delegate-Based Generic Optimization",
                    Success = true
                });
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Concurrent command processing failed");
                
                return BadRequest(new
                {
                    Error = ex.Message,
                    TotalProcessingTime = stopwatch.ElapsedMilliseconds,
                    Success = false
                });
            }
        }

        /// <summary>
        /// 测试类型安全的泛型处理
        /// </summary>
        [HttpPost("type-safe")]
        public async Task<IActionResult> TestTypeSafeProcessing([FromBody] TypeSafeTestRequest request)
        {
            var stopwatch = Stopwatch.StartNew();
            var results = new List<object>();
            
            _logger.LogInformation("Testing type-safe processing with {Count} different command types", request.Commands.Count);
            
            try
            {
                // 处理不同类型的命令，展示类型安全
                foreach (var cmd in request.Commands)
                {
                    if (cmd is DelegateBasedOrderCommand orderCmd)
                    {
                        var result = await _commandBus.SendAsync<DelegateBasedOrderCommand, DelegateBasedOrderResult>(orderCmd);
                        results.Add(new { Type = "OrderCommand", Result = result });
                    }
                    else if (cmd is DelegateBasedPaymentCommand paymentCmd)
                    {
                        var result = await _commandBus.SendAsync<DelegateBasedPaymentCommand, DelegateBasedPaymentResult>(paymentCmd);
                        results.Add(new { Type = "PaymentCommand", Result = result });
                    }
                }
                
                stopwatch.Stop();
                
                return Ok(new
                {
                    Results = results,
                    TotalProcessingTime = stopwatch.ElapsedMilliseconds,
                    CommandTypes = results.Select(r => ((dynamic)r).Type).Distinct(),
                    OptimizationType = "Delegate-Based Generic Optimization",
                    TypeSafety = "Maintained",
                    Success = true
                });
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Type-safe processing failed");
                
                return BadRequest(new
                {
                    Error = ex.Message,
                    TotalProcessingTime = stopwatch.ElapsedMilliseconds,
                    Success = false
                });
            }
        }

        /// <summary>
        /// 获取委托基础CommandBus的性能指标
        /// </summary>
        [HttpGet("metrics")]
        public IActionResult GetDelegateBasedMetrics()
        {
            try
            {
                if (_commandBus is DelegateBasedCommandBus delegateBasedBus)
                {
                    var metrics = delegateBasedBus.GetMetrics();
                    
                    return Ok(new
                    {
                        MaxConcurrency = metrics.MaxConcurrency,
                        InputQueueSize = metrics.InputQueueSize,
                        CachedHandlers = metrics.CachedHandlers,
                        OptimizationType = "Delegate-Based Generic Optimization",
                        TypeSafety = "Maintained",
                        Performance = "Optimized"
                    });
                }
                
                return Ok(new { Message = "Delegate-based CommandBus metrics not available for current implementation" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get delegate-based metrics");
                return BadRequest(new { Error = ex.Message });
            }
        }

        /// <summary>
        /// 重置委托基础CommandBus的性能指标
        /// </summary>
        [HttpPost("metrics/reset")]
        public IActionResult ResetDelegateBasedMetrics()
        {
            try
            {
                if (_commandBus is DelegateBasedCommandBus delegateBasedBus)
                {
                    delegateBasedBus.ClearCache();
                    return Ok(new { Message = "Delegate-based CommandBus metrics reset successfully" });
                }
                
                return Ok(new { Message = "Delegate-based CommandBus metrics reset not supported for current implementation" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to reset delegate-based metrics");
                return BadRequest(new { Error = ex.Message });
            }
        }
    }

    // 委托基础命令和结果模型
    public record DelegateBasedOrderCommand(string Product, int Quantity, int Priority = 1) : ICommand<DelegateBasedOrderResult>;

    public record DelegateBasedOrderResult(string RequestId, DateTime CreatedAt, string Product, int Quantity, int Priority, string Message);

    public record DelegateBasedPaymentCommand(string PaymentMethod, decimal Amount, string Currency = "USD") : ICommand<DelegateBasedPaymentResult>;

    public record DelegateBasedPaymentResult(string RequestId, DateTime CreatedAt, string PaymentMethod, decimal Amount, string Currency, string Message);

    public class DelegateBasedOrderHandler : ICommandHandler<DelegateBasedOrderCommand, DelegateBasedOrderResult>
    {
        private readonly ILogger<DelegateBasedOrderHandler> _logger;

        public DelegateBasedOrderHandler(ILogger<DelegateBasedOrderHandler> logger)
        {
            _logger = logger;
        }

        public async Task<DelegateBasedOrderResult> HandleAsync(DelegateBasedOrderCommand command, CancellationToken ct = default)
        {
            // 模拟一些处理时间
            var processingTime = Random.Shared.Next(10, 100);
            await Task.Delay(processingTime, ct);
            
            var requestId = $"{DateTime.UtcNow.Ticks}_{Guid.NewGuid():N}";
            var createdAt = DateTime.UtcNow;
            
            _logger.LogDebug("Processed delegate-based order: {Product} x {Quantity} (Priority: {Priority}) - RequestId: {RequestId}", 
                command.Product, command.Quantity, command.Priority, requestId);
            
            return new DelegateBasedOrderResult(
                requestId,
                createdAt,
                command.Product,
                command.Quantity,
                command.Priority,
                $"Delegate-based order processed: {command.Product} x {command.Quantity} (Priority: {command.Priority}) - Processing time: {processingTime}ms"
            );
        }
    }

    public class DelegateBasedPaymentHandler : ICommandHandler<DelegateBasedPaymentCommand, DelegateBasedPaymentResult>
    {
        private readonly ILogger<DelegateBasedPaymentHandler> _logger;

        public DelegateBasedPaymentHandler(ILogger<DelegateBasedPaymentHandler> logger)
        {
            _logger = logger;
        }

        public async Task<DelegateBasedPaymentResult> HandleAsync(DelegateBasedPaymentCommand command, CancellationToken ct = default)
        {
            // 模拟一些处理时间
            var processingTime = Random.Shared.Next(20, 150);
            await Task.Delay(processingTime, ct);
            
            var requestId = $"{DateTime.UtcNow.Ticks}_{Guid.NewGuid():N}";
            var createdAt = DateTime.UtcNow;
            
            _logger.LogDebug("Processed delegate-based payment: {PaymentMethod} {Amount} {Currency} - RequestId: {RequestId}", 
                command.PaymentMethod, command.Amount, command.Currency, requestId);
            
            return new DelegateBasedPaymentResult(
                requestId,
                createdAt,
                command.PaymentMethod,
                command.Amount,
                command.Currency,
                $"Delegate-based payment processed: {command.PaymentMethod} {command.Amount} {command.Currency} - Processing time: {processingTime}ms"
            );
        }
    }

    // 请求模型
    public class ConcurrentTestRequest
    {
        public int Count { get; set; } = 10;
        public int Quantity { get; set; } = 1;
        public int Priority { get; set; } = 1;
    }

    public class TypeSafeTestRequest
    {
        public List<object> Commands { get; set; } = new();
    }
}
