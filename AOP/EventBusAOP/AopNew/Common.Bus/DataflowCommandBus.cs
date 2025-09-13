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
    /// 基于TPL数据流的高性能CommandBus实现
    /// 支持并行处理、背压控制和监控
    /// </summary>
    public class DataflowCommandBus : ICommandBus, IDisposable
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<DataflowCommandBus>? _logger;
        private readonly ConcurrentDictionary<Type, Func<object>> _handlerCache = new();
        private readonly ConcurrentDictionary<Type, Func<object[]>> _behaviorsCache = new();
        
        // 数据流网络
        private ActionBlock<DataflowCommandRequest> _commandProcessor = null!;
        
        // 背压控制
        private readonly SemaphoreSlim _concurrencyLimiter;
        private readonly int _maxConcurrency;
        
        // 监控指标
        private long _processedCommands;
        private long _failedCommands;
        private long _totalProcessingTime;

        public DataflowCommandBus(IServiceProvider serviceProvider, ILogger<DataflowCommandBus>? logger = null, 
            int? maxConcurrency = null)
        {
            _provider = serviceProvider;
            _logger = logger;
            _maxConcurrency = maxConcurrency ?? Environment.ProcessorCount * 2;
            _concurrencyLimiter = new SemaphoreSlim(_maxConcurrency, _maxConcurrency);
            
            // 创建数据流网络
            CreateDataflowNetwork();
        }

        private void CreateDataflowNetwork()
        {
            // 创建命令处理器
            _commandProcessor = new ActionBlock<DataflowCommandRequest>(
                async request =>
                {
                    try
                    {
                        await _concurrencyLimiter.WaitAsync();
                        var startTime = DateTime.UtcNow;
                        
                        // 执行完整的命令处理管道
                        var result = await ProcessCommandPipeline(request);
                        
                        var processingTime = DateTime.UtcNow - startTime;
                        Interlocked.Add(ref _totalProcessingTime, processingTime.Ticks);
                        Interlocked.Increment(ref _processedCommands);
                        
                        request.TaskCompletionSource.SetResult(result);
                    }
                    catch (Exception ex)
                    {
                        Interlocked.Increment(ref _failedCommands);
                        _logger?.LogError(ex, "Command processing failed for {CommandType}", request.CommandType.Name);
                        request.TaskCompletionSource.SetException(ex);
                    }
                    finally
                    {
                        _concurrencyLimiter.Release();
                    }
                },
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = _maxConcurrency,
                    BoundedCapacity = _maxConcurrency * 2
                });
        }

        public async Task<TResult> SendAsync<TCommand, TResult>(TCommand command, CancellationToken ct = default) 
            where TCommand : ICommand<TResult>
        {
            var commandType = typeof(TCommand);
            var requestId = Guid.NewGuid();
            var tcs = new TaskCompletionSource<object>();
            
            var request = new DataflowCommandRequest(requestId, commandType, command, tcs);
            
            // 发送到数据流网络
            if (!_commandProcessor.Post(request))
            {
                throw new InvalidOperationException("Unable to queue command for processing - system may be overloaded");
            }
            
            try
            {
                var result = await tcs.Task.WaitAsync(ct);
                return (TResult)result;
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                _logger?.LogWarning("Command {CommandType} was cancelled", commandType.Name);
                throw;
            }
        }

        private async Task<object> ProcessCommandPipeline(DataflowCommandRequest request)
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
        public DataflowMetrics GetMetrics()
        {
            return new DataflowMetrics
            {
                ProcessedCommands = Interlocked.Read(ref _processedCommands),
                FailedCommands = Interlocked.Read(ref _failedCommands),
                TotalProcessingTime = TimeSpan.FromTicks(Interlocked.Read(ref _totalProcessingTime)),
                AverageProcessingTime = _processedCommands > 0 
                    ? TimeSpan.FromTicks(Interlocked.Read(ref _totalProcessingTime) / _processedCommands)
                    : TimeSpan.Zero,
                AvailableConcurrency = _concurrencyLimiter.CurrentCount,
                MaxConcurrency = _maxConcurrency,
                InputQueueSize = _commandProcessor.InputCount
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
            _concurrencyLimiter?.Dispose();
        }
    }

    // 辅助类
    internal class DataflowCommandRequest
    {
        public Guid Id { get; }
        public Type CommandType { get; }
        public object Command { get; }
        public TaskCompletionSource<object> TaskCompletionSource { get; }

        public DataflowCommandRequest(Guid id, Type commandType, object command, TaskCompletionSource<object> tcs)
        {
            Id = id;
            CommandType = commandType;
            Command = command;
            TaskCompletionSource = tcs;
        }
    }

    public class DataflowMetrics : IDataflowMetrics
    {
        public long ProcessedCommands { get; set; }
        public long FailedCommands { get; set; }
        public TimeSpan TotalProcessingTime { get; set; }
        public TimeSpan AverageProcessingTime { get; set; }
        public int AvailableConcurrency { get; set; }
        public int MaxConcurrency { get; set; }
        public int InputQueueSize { get; set; }
        public double SuccessRate => ProcessedCommands + FailedCommands > 0 
            ? (double)ProcessedCommands / (ProcessedCommands + FailedCommands) * 100 
            : 0;
    }
}