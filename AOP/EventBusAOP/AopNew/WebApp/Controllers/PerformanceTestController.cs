using Common.Bus;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace WebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PerformanceTestController : ControllerBase
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PerformanceTestController> _logger;

        public PerformanceTestController(IServiceProvider serviceProvider, ILogger<PerformanceTestController> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        /// <summary>
        /// 性能测试：比较不同CommandBus实现的性能
        /// </summary>
        [HttpPost("compare")]
        public async Task<IActionResult> CompareCommandBusPerformance([FromBody] PerformanceTestRequest request)
        {
            var results = new List<PerformanceTestResult>();
            
            // 测试标准CommandBus
            var standardBus = new CommandBus(_serviceProvider);
            var standardResult = await RunPerformanceTest("Standard CommandBus", standardBus, request);
            results.Add(standardResult);
            
            // 测试数据流CommandBus
            var dataflowBus = new DataflowCommandBus(_serviceProvider, null, request.MaxConcurrency);
            var dataflowResult = await RunPerformanceTest("Dataflow CommandBus", dataflowBus, request);
            results.Add(dataflowResult);
            
            // 测试批处理数据流CommandBus
            var batchBus = new BatchDataflowCommandBus(_serviceProvider, null, request.BatchSize, 
                TimeSpan.FromMilliseconds(request.BatchTimeout), request.MaxConcurrency);
            var batchResult = await RunPerformanceTest("Batch Dataflow CommandBus", batchBus, request);
            results.Add(batchResult);
            
            // 清理资源
            dataflowBus.Dispose();
            batchBus.Dispose();
            
            return Ok(new
            {
                TestConfiguration = new
                {
                    request.CommandCount,
                    request.MaxConcurrency,
                    request.BatchSize,
                    request.BatchTimeout
                },
                Results = results.OrderBy(r => r.TotalTime).ToList(),
                Summary = new
                {
                    Fastest = results.OrderBy(r => r.TotalTime).First().Implementation,
                    Slowest = results.OrderByDescending(r => r.TotalTime).First().Implementation,
                    AverageThroughput = results.Average(r => r.Throughput),
                    BestThroughput = results.Max(r => r.Throughput)
                }
            });
        }

        private async Task<PerformanceTestResult> RunPerformanceTest(string implementation, ICommandBus commandBus, PerformanceTestRequest request)
        {
            _logger.LogInformation("Starting performance test for {Implementation}", implementation);
            
            var stopwatch = Stopwatch.StartNew();
            var tasks = new List<Task<string>>();
            var errors = 0;
            
            // 创建并发任务
            for (int i = 0; i < request.CommandCount; i++)
            {
                var command = new ProcessOrderCommand($"TestProduct-{i}", 1, Random.Shared.Next(1, 5));
                tasks.Add(ExecuteCommandWithErrorHandling(commandBus, command, () => Interlocked.Increment(ref errors)));
            }
            
            try
            {
                var results = await Task.WhenAll(tasks);
                stopwatch.Stop();
                
                var successCount = request.CommandCount - errors;
                var throughput = request.CommandCount / stopwatch.Elapsed.TotalSeconds;
                
                _logger.LogInformation("Completed performance test for {Implementation}: {ElapsedMs}ms, {Throughput:F2} commands/sec", 
                    implementation, stopwatch.ElapsedMilliseconds, throughput);
                
                return new PerformanceTestResult
                {
                    Implementation = implementation,
                    TotalTime = stopwatch.ElapsedMilliseconds,
                    CommandCount = request.CommandCount,
                    SuccessCount = successCount,
                    ErrorCount = errors,
                    SuccessRate = (double)successCount / request.CommandCount * 100,
                    Throughput = throughput,
                    AverageTimePerCommand = stopwatch.ElapsedMilliseconds / (double)request.CommandCount
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Performance test failed for {Implementation}", implementation);
                
                return new PerformanceTestResult
                {
                    Implementation = implementation,
                    TotalTime = stopwatch.ElapsedMilliseconds,
                    CommandCount = request.CommandCount,
                    SuccessCount = 0,
                    ErrorCount = request.CommandCount,
                    SuccessRate = 0,
                    Throughput = 0,
                    AverageTimePerCommand = stopwatch.ElapsedMilliseconds / (double)request.CommandCount,
                    Error = ex.Message
                };
            }
        }

        private async Task<string> ExecuteCommandWithErrorHandling(ICommandBus commandBus, ProcessOrderCommand command, Action onError)
        {
            try
            {
                return await commandBus.SendAsync<ProcessOrderCommand, string>(command);
            }
            catch
            {
                onError();
                throw;
            }
        }

        /// <summary>
        /// 压力测试：测试系统在高负载下的表现
        /// </summary>
        [HttpPost("stress")]
        public async Task<IActionResult> StressTest([FromBody] StressTestRequest request)
        {
            var commandBus = new DataflowCommandBus(_serviceProvider, null, request.MaxConcurrency);
            
            try
            {
                var stopwatch = Stopwatch.StartNew();
                var tasks = new List<Task<string>>();
                var completedTasks = 0;
                var failedTasks = 0;
                
                _logger.LogInformation("Starting stress test with {CommandCount} commands", request.CommandCount);
                
                // 分批创建任务以避免内存问题
                const int batchSize = 1000;
                for (int batch = 0; batch < request.CommandCount; batch += batchSize)
                {
                    var currentBatchSize = Math.Min(batchSize, request.CommandCount - batch);
                    var batchTasks = new List<Task<string>>();
                    
                    for (int i = 0; i < currentBatchSize; i++)
                    {
                        var command = new ProcessOrderCommand($"StressProduct-{batch + i}", 1, Random.Shared.Next(1, 5));
                        batchTasks.Add(ExecuteCommandWithErrorHandling(commandBus, command, () => Interlocked.Increment(ref failedTasks)));
                    }
                    
                    tasks.AddRange(batchTasks);
                    
                    // 如果任务太多，等待一些完成
                    if (tasks.Count > request.MaxConcurrency * 10)
                    {
                        var completed = await Task.WhenAny(tasks);
                        tasks.Remove(completed);
                        Interlocked.Increment(ref completedTasks);
                    }
                }
                
                // 等待所有任务完成
                var results = await Task.WhenAll(tasks);
                stopwatch.Stop();
                
                completedTasks += results.Length;
                
                _logger.LogInformation("Completed stress test: {ElapsedMs}ms, {Completed} completed, {Failed} failed", 
                    stopwatch.ElapsedMilliseconds, completedTasks, failedTasks);
                
                return Ok(new
                {
                    TotalTime = stopwatch.ElapsedMilliseconds,
                    CommandCount = request.CommandCount,
                    CompletedCount = completedTasks,
                    FailedCount = failedTasks,
                    SuccessRate = (double)completedTasks / request.CommandCount * 100,
                    Throughput = request.CommandCount / stopwatch.Elapsed.TotalSeconds,
                    AverageTimePerCommand = stopwatch.ElapsedMilliseconds / (double)request.CommandCount
                });
            }
            finally
            {
                commandBus.Dispose();
            }
        }
    }

    // 请求和结果模型
    public class PerformanceTestRequest
    {
        public int CommandCount { get; set; } = 100;
        public int MaxConcurrency { get; set; } = Environment.ProcessorCount * 2;
        public int BatchSize { get; set; } = 10;
        public int BatchTimeout { get; set; } = 100;
    }

    public class StressTestRequest
    {
        public int CommandCount { get; set; } = 1000;
        public int MaxConcurrency { get; set; } = Environment.ProcessorCount * 2;
    }

    public class PerformanceTestResult
    {
        public string Implementation { get; set; } = string.Empty;
        public long TotalTime { get; set; }
        public int CommandCount { get; set; }
        public int SuccessCount { get; set; }
        public int ErrorCount { get; set; }
        public double SuccessRate { get; set; }
        public double Throughput { get; set; }
        public double AverageTimePerCommand { get; set; }
        public string? Error { get; set; }
    }
}
