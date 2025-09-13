using Common.Bus.Core;
using Common.Bus.Implementations;
using Common.Bus.Monitoring;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApp.Handlers.Commands;

namespace WebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataflowDemoController : ControllerBase
    {
        private readonly ICommandBus _commandBus;
        private readonly ILogger<DataflowDemoController> _logger;

        public DataflowDemoController(ICommandBus commandBus, ILogger<DataflowDemoController> logger)
        {
            _commandBus = commandBus;
            _logger = logger;
        }

        /// <summary>
        /// 测试单个命令处理
        /// </summary>
        [HttpPost("single")]
        public async Task<IActionResult> ProcessSingleCommand([FromBody] ProcessOrderCommand command)
        {
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                var result = await _commandBus.SendAsync<ProcessOrderCommand, string>(command);
                stopwatch.Stop();
                
                return Ok(new
                {
                    Result = result,
                    ProcessingTime = stopwatch.ElapsedMilliseconds,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Command processing failed");
                
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
            var tasks = new List<Task<string>>();
            
            _logger.LogInformation("Starting concurrent processing of {Count} commands", request.Count);
            
            // 创建并发任务
            for (int i = 0; i < request.Count; i++)
            {
                var command = new ProcessOrderCommand($"Product-{i}", request.Quantity, request.Priority);
                tasks.Add(_commandBus.SendAsync<ProcessOrderCommand, string>(command));
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
        /// 测试批处理命令
        /// </summary>
        [HttpPost("batch")]
        public async Task<IActionResult> ProcessBatchCommands([FromBody] BatchTestRequest request)
        {
            var stopwatch = Stopwatch.StartNew();
            var tasks = new List<Task<string>>();
            
            _logger.LogInformation("Starting batch processing of {Count} commands", request.Commands.Count);
            
            // 创建批处理任务
            foreach (var cmd in request.Commands)
            {
                tasks.Add(_commandBus.SendAsync<ProcessOrderCommand, string>(cmd));
            }
            
            try
            {
                var results = await Task.WhenAll(tasks);
                stopwatch.Stop();
                
                _logger.LogInformation("Completed batch processing in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
                
                return Ok(new
                {
                    Results = results,
                    TotalProcessingTime = stopwatch.ElapsedMilliseconds,
                    AverageProcessingTime = stopwatch.ElapsedMilliseconds / (double)request.Commands.Count,
                    Throughput = request.Commands.Count / stopwatch.Elapsed.TotalSeconds,
                    BatchSize = request.Commands.Count,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Batch command processing failed");
                
                return BadRequest(new
                {
                    Error = ex.Message,
                    TotalProcessingTime = stopwatch.ElapsedMilliseconds,
                    Success = false
                });
            }
        }

        /// <summary>
        /// 获取系统性能指标
        /// </summary>
        [HttpGet("metrics")]
        public IActionResult GetMetrics()
        {
            try
            {
                if (_commandBus is TypedDataflowCommandBus typedBus)
                {
                    var metrics = typedBus.GetMetrics();
                    
                    return Ok(new
                    {
                        ProcessedCommands = metrics.ProcessedCommands,
                        FailedCommands = metrics.FailedCommands,
                        SuccessRate = metrics.SuccessRate,
                        AverageProcessingTime = metrics.AverageProcessingTime.TotalMilliseconds,
                        TotalProcessingTime = metrics.TotalProcessingTime.TotalMilliseconds,
                        AvailableConcurrency = metrics.AvailableConcurrency,
                        MaxConcurrency = metrics.MaxConcurrency,
                        InputQueueSize = metrics.InputQueueSize
                    });
                }
                else if (_commandBus is IMonitoredCommandBus monitoredBus)
                {
                    var metrics = monitoredBus.GetMetrics();
                    
                    return Ok(new
                    {
                        ProcessedCommands = metrics.ProcessedCommands,
                        FailedCommands = metrics.FailedCommands,
                        SuccessRate = metrics.SuccessRate,
                        AverageProcessingTime = metrics.AverageProcessingTime.TotalMilliseconds,
                        TotalProcessingTime = metrics.TotalProcessingTime.TotalMilliseconds
                    });
                }
                else if (_commandBus is DataflowCommandBus dataflowBus)
                {
                    var metrics = dataflowBus.GetMetrics();
                    
                    return Ok(new
                    {
                        ProcessedCommands = metrics.ProcessedCommands,
                        FailedCommands = metrics.FailedCommands,
                        SuccessRate = metrics.SuccessRate,
                        AverageProcessingTime = metrics.AverageProcessingTime.TotalMilliseconds,
                        TotalProcessingTime = metrics.TotalProcessingTime.TotalMilliseconds,
                        AvailableConcurrency = metrics.AvailableConcurrency,
                        MaxConcurrency = metrics.MaxConcurrency,
                        InputQueueSize = metrics.InputQueueSize
                    });
                }
                else if (_commandBus is BatchDataflowCommandBus batchBus)
                {
                    var metrics = batchBus.GetMetrics();
                    
                    return Ok(new
                    {
                        ProcessedCommands = metrics.ProcessedCommands,
                        FailedCommands = metrics.FailedCommands,
                        SuccessRate = metrics.SuccessRate,
                        AverageProcessingTime = metrics.AverageProcessingTime.TotalMilliseconds,
                        TotalProcessingTime = metrics.TotalProcessingTime.TotalMilliseconds,
                        ProcessedBatches = metrics.ProcessedBatches,
                        AverageBatchSize = metrics.AverageBatchSize,
                        Throughput = metrics.Throughput,
                        BatchSize = metrics.BatchSize,
                        BatchTimeout = metrics.BatchTimeout.TotalMilliseconds
                    });
                }
                
                return Ok(new { Message = "Metrics not available for current CommandBus implementation" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get metrics");
                return BadRequest(new { Error = ex.Message });
            }
        }

        /// <summary>
        /// 重置性能指标
        /// </summary>
        [HttpPost("metrics/reset")]
        public IActionResult ResetMetrics()
        {
            try
            {
                if (_commandBus is IMonitoredCommandBus monitoredBus)
                {
                    monitoredBus.ResetMetrics();
                    return Ok(new { Message = "Metrics reset successfully" });
                }
                
                return Ok(new { Message = "Metrics reset not supported for current CommandBus implementation" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to reset metrics");
                return BadRequest(new { Error = ex.Message });
            }
        }
    }

    

  

    public class ConcurrentTestRequest
    {
        public int Count { get; set; } = 10;
        public int Quantity { get; set; } = 1;
        public int Priority { get; set; } = 1;
    }

    public class BatchTestRequest
    {
        public List<ProcessOrderCommand> Commands { get; set; } = new();
    }
}
