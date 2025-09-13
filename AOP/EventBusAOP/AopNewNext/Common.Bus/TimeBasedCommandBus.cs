using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Common.Bus
{
    /// <summary>
    /// 基于时间戳的优化CommandBus实现
    /// 简化泛型打包的复杂性，提供更好的性能和监控能力
    /// </summary>
    public class TimeBasedCommandBus : ICommandBus, IDisposable
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<TimeBasedCommandBus>? _logger;
        private readonly TimeWindowManager _timeWindowManager;
        private readonly ConcurrentDictionary<Type, Func<object>> _handlerCache = new();
        private readonly ConcurrentDictionary<Type, Func<object[]>> _behaviorsCache = new();
        
        // 数据流网络
        private ActionBlock<TimeBasedCommandRequest> _commandProcessor = null!;
        private ActionBlock<List<TimeBasedCommandRequest>> _batchProcessor = null!;
        
        // 配置参数
        private readonly int _maxConcurrency;
        private readonly bool _enableBatchProcessing;
        private readonly TimeSpan _batchWindowSize;
        
        // 监控指标
        private long _processedCommands;
        private long _failedCommands;
        private long _totalProcessingTime;
        private long _processedBatches;

        public TimeBasedCommandBus(IServiceProvider serviceProvider, ILogger<TimeBasedCommandBus>? logger = null,
            int? maxConcurrency = null, bool enableBatchProcessing = true, TimeSpan? batchWindowSize = null)
        {
            _provider = serviceProvider;
            _logger = logger;
            _maxConcurrency = maxConcurrency ?? Environment.ProcessorCount * 2;
            _enableBatchProcessing = enableBatchProcessing;
            _batchWindowSize = batchWindowSize ?? TimeSpan.FromMilliseconds(50);
            
            // 创建时间窗口管理器
            var timeWindowLogger = _provider.GetService<ILogger<TimeWindowManager>>();
            _timeWindowManager = new TimeWindowManager(_batchWindowSize, timeWindowLogger);
            
            // 创建数据流网络
            CreateDataflowNetwork();
        }

        private void CreateDataflowNetwork()
        {
            // 创建命令处理器
            _commandProcessor = new ActionBlock<TimeBasedCommandRequest>(
                async request =>
                {
                    try
                    {
                        var startTime = DateTime.UtcNow;
                        
                        // 执行完整的命令处理管道
                        var result = await ProcessCommandPipeline(request);
                        
                        var processingTime = DateTime.UtcNow - startTime;
                        Interlocked.Add(ref _totalProcessingTime, processingTime.Ticks);
                        Interlocked.Increment(ref _processedCommands);
                        
                        request.SetResult(result);
                        
                        _logger?.LogDebug("Processed command {RequestId} in {ProcessingTime}ms", 
                            request.RequestId, processingTime.TotalMilliseconds);
                    }
                    catch (Exception ex)
                    {
                        Interlocked.Increment(ref _failedCommands);
                        _logger?.LogError(ex, "Command processing failed for {RequestId}", request.RequestId);
                        request.SetException(ex);
                    }
                },
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = _maxConcurrency,
                    BoundedCapacity = _maxConcurrency * 2
                });

            // 创建批处理器
            _batchProcessor = new ActionBlock<List<TimeBasedCommandRequest>>(
                async batch =>
                {
                    try
                    {
                        var startTime = DateTime.UtcNow;
                        
                        _logger?.LogDebug("Processing batch of {Count} commands", batch.Count);
                        
                        // 并行处理批次中的命令
                        var tasks = batch.Select(async request =>
                        {
                            try
                            {
                                var result = await ProcessCommandPipeline(request);
                                request.SetResult(result);
                                Interlocked.Increment(ref _processedCommands);
                            }
                            catch (Exception ex)
                            {
                                _logger?.LogError(ex, "Command processing failed for {RequestId}", request.RequestId);
                                request.SetException(ex);
                                Interlocked.Increment(ref _failedCommands);
                            }
                        });
                        
                        await Task.WhenAll(tasks);
                        
                        var processingTime = DateTime.UtcNow - startTime;
                        Interlocked.Add(ref _totalProcessingTime, processingTime.Ticks);
                        Interlocked.Increment(ref _processedBatches);
                        
                        _logger?.LogDebug("Completed batch processing in {ProcessingTime}ms", 
                            processingTime.TotalMilliseconds);
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, "Batch processing failed");
                        
                        // 设置所有请求为失败
                        foreach (var request in batch)
                        {
                            request.SetException(ex);
                        }
                    }
                },
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = _maxConcurrency,
                    BoundedCapacity = 2
                });

            // 设置时间窗口处理事件
            if (_enableBatchProcessing)
            {
                _timeWindowManager.OnTimeWindowReady += requests =>
                {
                    if (requests.Count > 0)
                    {
                        _batchProcessor.Post(requests);
                    }
                };
            }
        }

        public async Task<TResult> SendAsync<TCommand, TResult>(TCommand command, CancellationToken ct = default) 
            where TCommand : ICommand<TResult>
        {
            var request = new TimeBasedCommandRequest(command);
            
            if (_enableBatchProcessing)
            {
                // 添加到时间窗口进行批处理
                _timeWindowManager.AddRequest(request);
            }
            else
            {
                // 直接发送到命令处理器
                if (!_commandProcessor.Post(request))
                {
                    throw new InvalidOperationException("Unable to queue command for processing - system may be overloaded");
                }
            }
            
            try
            {
                var result = await request.ExecuteAsync(ct);
                return (TResult)result;
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                _logger?.LogWarning("Command {RequestId} was cancelled", request.RequestId);
                throw;
            }
        }

        private async Task<object> ProcessCommandPipeline(TimeBasedCommandRequest request)
        {
            // 获取处理器和管道行为的工厂函数
            var handlerFactory = GetCachedHandler(request.CommandType);
            var behaviorsFactory = GetCachedBehaviors(request.CommandType);
            
            // 创建处理器和行为的实例
            var handler = handlerFactory();
            var behaviors = behaviorsFactory();
            
            // 构建处理管道
            Func<Task<object>> pipeline = () => ExecuteHandler(handler, request.Command);
            
            // 按顺序应用管道行为
            foreach (var behavior in behaviors.Reverse())
            {
                var currentBehavior = behavior;
                var currentPipeline = pipeline;
                pipeline = () => ExecuteBehavior(currentBehavior, request.Command, currentPipeline);
            }
            
            return await pipeline();
        }

        private async Task<object> ExecuteBehavior(object behavior, object command, Func<Task<object>> next)
        {
            var behaviorType = behavior.GetType();
            var handleMethod = behaviorType.GetMethod("Handle");
            
            if (handleMethod == null)
                throw new InvalidOperationException($"Behavior {behaviorType.Name} does not have Handle method");

            // 使用动态类型来避免泛型类型转换问题
            dynamic dynamicBehavior = behavior;
            dynamic dynamicCommand = command;
            
            // 创建一个包装的next函数，返回object类型
            Func<Task<object>> wrappedNext = async () => await next();
            
            try
            {
                // 使用动态调用，让运行时处理类型转换
                var result = await dynamicBehavior.Handle(dynamicCommand, wrappedNext, CancellationToken.None);
                return result;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error executing behavior {behaviorType.Name}: {ex.Message}", ex);
            }
        }

        private async Task<object> ExecuteHandler(object handler, object command)
        {
            var handlerType = handler.GetType();
            var handleMethod = handlerType.GetMethod("HandleAsync");
            
            if (handleMethod == null)
                throw new InvalidOperationException($"Handler {handlerType.Name} does not have HandleAsync method");

            var task = (Task)handleMethod.Invoke(handler, new object[] { command, CancellationToken.None });
            await task;
            
            var resultProperty = task.GetType().GetProperty("Result");
            return resultProperty?.GetValue(task);
        }

        private Func<object> GetCachedHandler(Type commandType)
        {
            return _handlerCache.GetOrAdd(commandType, _ =>
            {
                // 获取命令类型实现的ICommand<TResult>接口
                var commandInterface = commandType.GetInterfaces()
                    .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommand<>));
                
                if (commandInterface == null)
                    throw new InvalidOperationException($"Command type {commandType.Name} does not implement ICommand<TResult>");
                
                var resultType = commandInterface.GetGenericArguments()[0];
                var handlerType = typeof(ICommandHandler<,>).MakeGenericType(commandType, resultType);
                
                // 返回一个工厂函数，而不是直接返回处理器实例
                return new Func<object>(() =>
                {
                    using var scope = _provider.CreateScope();
                    var handler = scope.ServiceProvider.GetService(handlerType);
                    if (handler == null)
                        throw new InvalidOperationException($"No handler registered for {commandType.Name}");
                    return handler;
                });
            });
        }

        private Func<object[]> GetCachedBehaviors(Type commandType)
        {
            return _behaviorsCache.GetOrAdd(commandType, _ =>
            {
                // 获取命令类型实现的ICommand<TResult>接口
                var commandInterface = commandType.GetInterfaces()
                    .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommand<>));
                
                if (commandInterface == null)
                    throw new InvalidOperationException($"Command type {commandType.Name} does not implement ICommand<TResult>");
                
                var resultType = commandInterface.GetGenericArguments()[0];
                var behaviorType = typeof(ICommandPipelineBehavior<,>).MakeGenericType(commandType, resultType);
                
                // 返回一个工厂函数，而不是直接返回行为实例
                return new Func<object[]>(() =>
                {
                    using var scope = _provider.CreateScope();
                    var behaviors = scope.ServiceProvider.GetServices(behaviorType).Where(b => b != null).ToArray();
                    return behaviors!;
                });
            });
        }

        // 监控和统计方法
        public TimeBasedCommandBusMetrics GetMetrics()
        {
            var timeWindowMetrics = _timeWindowManager.GetMetrics();
            
            return new TimeBasedCommandBusMetrics
            {
                ProcessedCommands = Interlocked.Read(ref _processedCommands),
                FailedCommands = Interlocked.Read(ref _failedCommands),
                ProcessedBatches = Interlocked.Read(ref _processedBatches),
                TotalProcessingTime = TimeSpan.FromTicks(Interlocked.Read(ref _totalProcessingTime)),
                AverageProcessingTime = _processedCommands > 0 
                    ? TimeSpan.FromTicks(Interlocked.Read(ref _totalProcessingTime) / _processedCommands)
                    : TimeSpan.Zero,
                MaxConcurrency = _maxConcurrency,
                EnableBatchProcessing = _enableBatchProcessing,
                BatchWindowSize = _batchWindowSize,
                TimeWindowMetrics = timeWindowMetrics,
                InputQueueSize = _commandProcessor.InputCount + _batchProcessor.InputCount
            };
        }

        public void ClearCache()
        {
            _handlerCache.Clear();
            _behaviorsCache.Clear();
        }

        public void Dispose()
        {
            _commandProcessor?.Complete();
            _batchProcessor?.Complete();
            _timeWindowManager?.Dispose();
        }
    }

    /// <summary>
    /// 基于时间的CommandBus统计信息
    /// </summary>
    public class TimeBasedCommandBusMetrics
    {
        public long ProcessedCommands { get; set; }
        public long FailedCommands { get; set; }
        public long ProcessedBatches { get; set; }
        public TimeSpan TotalProcessingTime { get; set; }
        public TimeSpan AverageProcessingTime { get; set; }
        public int MaxConcurrency { get; set; }
        public bool EnableBatchProcessing { get; set; }
        public TimeSpan BatchWindowSize { get; set; }
        public TimeWindowMetrics TimeWindowMetrics { get; set; } = new();
        public int InputQueueSize { get; set; }
        
        public double SuccessRate => ProcessedCommands + FailedCommands > 0 
            ? (double)ProcessedCommands / (ProcessedCommands + FailedCommands) * 100 
            : 0;
        
        public double Throughput => TotalProcessingTime.TotalSeconds > 0 
            ? ProcessedCommands / TotalProcessingTime.TotalSeconds 
            : 0;
        
        public double AverageBatchSize => ProcessedBatches > 0 
            ? (double)ProcessedCommands / ProcessedBatches 
            : 0;
    }
}
