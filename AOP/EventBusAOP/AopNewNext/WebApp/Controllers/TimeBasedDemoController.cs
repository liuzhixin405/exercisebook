using Common.Bus;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace WebApp.Controllers
{
    /// <summary>
    /// 基于时间戳的CommandBus演示控制器
    /// 展示如何通过时间参数简化泛型打包请求
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TimeBasedDemoController : ControllerBase
    {
        private readonly ICommandBus _commandBus;
        private readonly ILogger<TimeBasedDemoController> _logger;

        public TimeBasedDemoController(ICommandBus commandBus, ILogger<TimeBasedDemoController> logger)
        {
            _commandBus = commandBus;
            _logger = logger;
        }

        /// <summary>
        /// 测试单个命令处理（时间戳优化版本）
        /// </summary>
        [HttpPost("single")]
        public async Task<IActionResult> ProcessSingleCommand([FromBody] TimeBasedOrderCommand command)
        {
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                // 使用时间戳优化的CommandBus，泛型参数更简洁
                var result = await _commandBus.SendAsync<TimeBasedOrderCommand, TimeBasedOrderResult>(command);
                stopwatch.Stop();
                
                return Ok(new
                {
                    Result = result,
                    ProcessingTime = stopwatch.ElapsedMilliseconds,
                    RequestId = result.RequestId,
                    CreatedAt = result.CreatedAt,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Time-based command processing failed");
                
                return BadRequest(new
                {
                    Error = ex.Message,
                    ProcessingTime = stopwatch.ElapsedMilliseconds,
                    Success = false
                });
            }
        }

        /// <summary>
        /// 测试时间窗口批处理
        /// </summary>
        [HttpPost("batch-window")]
        public async Task<IActionResult> ProcessBatchWindow([FromBody] BatchWindowTestRequest request)
        {
            var stopwatch = Stopwatch.StartNew();
            var tasks = new List<Task<TimeBasedOrderResult>>();
            
            _logger.LogInformation("Starting time-window batch processing of {Count} commands", request.Count);
            
            // 创建多个命令，它们会在相同的时间窗口内被批处理
            for (int i = 0; i < request.Count; i++)
            {
                var command = new TimeBasedOrderCommand($"Product-{i}", request.Quantity, request.Priority);
                tasks.Add(_commandBus.SendAsync<TimeBasedOrderCommand, TimeBasedOrderResult>(command));
            }
            
            try
            {
                var results = await Task.WhenAll(tasks);
                stopwatch.Stop();
                
                _logger.LogInformation("Completed time-window batch processing in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
                
                return Ok(new
                {
                    Results = results,
                    TotalProcessingTime = stopwatch.ElapsedMilliseconds,
                    AverageProcessingTime = stopwatch.ElapsedMilliseconds / (double)request.Count,
                    Throughput = request.Count / stopwatch.Elapsed.TotalSeconds,
                    BatchSize = request.Count,
                    TimeWindowOptimization = "Enabled",
                    Success = true
                });
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Time-window batch processing failed");
                
                return BadRequest(new
                {
                    Error = ex.Message,
                    TotalProcessingTime = stopwatch.ElapsedMilliseconds,
                    Success = false
                });
            }
        }

        /// <summary>
        /// 测试时间戳排序和优先级
        /// </summary>
        [HttpPost("priority-test")]
        public async Task<IActionResult> TestTimeBasedPriority([FromBody] PriorityTestRequest request)
        {
            var stopwatch = Stopwatch.StartNew();
            var tasks = new List<Task<TimeBasedOrderResult>>();
            
            _logger.LogInformation("Testing time-based priority with {Count} commands", request.Commands.Count);
            
            // 按不同优先级创建命令
            foreach (var cmd in request.Commands)
            {
                tasks.Add(_commandBus.SendAsync<TimeBasedOrderCommand, TimeBasedOrderResult>(cmd));
            }
            
            try
            {
                var results = await Task.WhenAll(tasks);
                stopwatch.Stop();
                
                // 按创建时间排序，展示时间戳优先级
                var sortedResults = results.OrderBy(r => r.CreatedAt).ToList();
                
                return Ok(new
                {
                    Results = sortedResults,
                    TotalProcessingTime = stopwatch.ElapsedMilliseconds,
                    TimeBasedOrdering = sortedResults.Select(r => new { 
                        r.RequestId, 
                        r.CreatedAt, 
                        r.Product 
                    }),
                    Success = true
                });
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Time-based priority test failed");
                
                return BadRequest(new
                {
                    Error = ex.Message,
                    TotalProcessingTime = stopwatch.ElapsedMilliseconds,
                    Success = false
                });
            }
        }

        /// <summary>
        /// 获取时间基础CommandBus的性能指标
        /// </summary>
        [HttpGet("metrics")]
        public IActionResult GetTimeBasedMetrics()
        {
            try
            {
                if (_commandBus is TimeBasedCommandBus timeBasedBus)
                {
                    var metrics = timeBasedBus.GetMetrics();
                    
                    return Ok(new
                    {
                        ProcessedCommands = metrics.ProcessedCommands,
                        FailedCommands = metrics.FailedCommands,
                        ProcessedBatches = metrics.ProcessedBatches,
                        SuccessRate = metrics.SuccessRate,
                        AverageProcessingTime = metrics.AverageProcessingTime.TotalMilliseconds,
                        TotalProcessingTime = metrics.TotalProcessingTime.TotalMilliseconds,
                        Throughput = metrics.Throughput,
                        AverageBatchSize = metrics.AverageBatchSize,
                        MaxConcurrency = metrics.MaxConcurrency,
                        EnableBatchProcessing = metrics.EnableBatchProcessing,
                        BatchWindowSize = metrics.BatchWindowSize.TotalMilliseconds,
                        TimeWindowMetrics = new
                        {
                            WindowSizeMs = metrics.TimeWindowMetrics.WindowSize.TotalMilliseconds,
                            ActiveWindows = metrics.TimeWindowMetrics.ActiveWindows,
                            TotalPendingRequests = metrics.TimeWindowMetrics.TotalPendingRequests,
                            OldestRequestAgeMs = metrics.TimeWindowMetrics.OldestRequestAge.TotalMilliseconds,
                            NewestRequestAgeMs = metrics.TimeWindowMetrics.NewestRequestAge.TotalMilliseconds,
                            AverageRequestsPerWindow = metrics.TimeWindowMetrics.AverageRequestsPerWindow
                        },
                        InputQueueSize = metrics.InputQueueSize
                    });
                }
                
                return Ok(new { Message = "Time-based CommandBus metrics not available for current implementation" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get time-based metrics");
                return BadRequest(new { Error = ex.Message });
            }
        }

        /// <summary>
        /// 重置时间基础CommandBus的性能指标
        /// </summary>
        [HttpPost("metrics/reset")]
        public IActionResult ResetTimeBasedMetrics()
        {
            try
            {
                if (_commandBus is TimeBasedCommandBus timeBasedBus)
                {
                    timeBasedBus.ClearCache();
                    return Ok(new { Message = "Time-based CommandBus metrics reset successfully" });
                }
                
                return Ok(new { Message = "Time-based CommandBus metrics reset not supported for current implementation" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to reset time-based metrics");
                return BadRequest(new { Error = ex.Message });
            }
        }
    }

    // 时间基础命令和结果模型
    public record TimeBasedOrderCommand(string Product, int Quantity, int Priority = 1) : ICommand<TimeBasedOrderResult>;

    public record TimeBasedOrderResult(string RequestId, DateTime CreatedAt, string Product, int Quantity, int Priority, string Message);

    public class TimeBasedOrderHandler : ICommandHandler<TimeBasedOrderCommand, TimeBasedOrderResult>
    {
        private readonly ILogger<TimeBasedOrderHandler> _logger;

        public TimeBasedOrderHandler(ILogger<TimeBasedOrderHandler> logger)
        {
            _logger = logger;
        }

        public async Task<TimeBasedOrderResult> HandleAsync(TimeBasedOrderCommand command, CancellationToken ct = default)
        {
            // 模拟一些处理时间
            var processingTime = Random.Shared.Next(10, 100);
            await Task.Delay(processingTime, ct);
            
            var requestId = $"{DateTime.UtcNow.Ticks}_{Guid.NewGuid():N}";
            var createdAt = DateTime.UtcNow;
            
            _logger.LogDebug("Processed time-based order: {Product} x {Quantity} (Priority: {Priority}) - RequestId: {RequestId}", 
                command.Product, command.Quantity, command.Priority, requestId);
            
            return new TimeBasedOrderResult(
                requestId,
                createdAt,
                command.Product,
                command.Quantity,
                command.Priority,
                $"Time-based order processed: {command.Product} x {command.Quantity} (Priority: {command.Priority}) - Processing time: {processingTime}ms"
            );
        }
    }

    // 请求模型
    public class BatchWindowTestRequest
    {
        public int Count { get; set; } = 10;
        public int Quantity { get; set; } = 1;
        public int Priority { get; set; } = 1;
    }

    public class PriorityTestRequest
    {
        public List<TimeBasedOrderCommand> Commands { get; set; } = new();
    }
}
