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
    /// 基于委托的泛型优化CommandBus实现
    /// 通过委托简化泛型打包，保持类型安全
    /// </summary>
    public class DelegateBasedCommandBus : ICommandBus, IDisposable
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<DelegateBasedCommandBus>? _logger;
        private readonly ConcurrentDictionary<Type, Func<object, CancellationToken, Task<object>>> _handlerCache = new();
        private readonly ConcurrentDictionary<Type, Func<object, CancellationToken, Task<object>>> _pipelineCache = new();
        
        // 数据流网络
        private ActionBlock<DelegateCommandRequest> _commandProcessor = null!;
        
        // 配置参数
        private readonly int _maxConcurrency;

        public DelegateBasedCommandBus(IServiceProvider serviceProvider, ILogger<DelegateBasedCommandBus>? logger = null,
            int? maxConcurrency = null)
        {
            _provider = serviceProvider;
            _logger = logger;
            _maxConcurrency = maxConcurrency ?? Environment.ProcessorCount * 2;
            
            // 创建数据流网络
            CreateDataflowNetwork();
        }

        private void CreateDataflowNetwork()
        {
            // 创建命令处理器
            _commandProcessor = new ActionBlock<DelegateCommandRequest>(
                async request =>
                {
                    try
                    {
                        var startTime = DateTime.UtcNow;
                        
                        // 执行命令处理管道
                        var result = await ProcessCommandPipeline(request);
                        
                        var processingTime = DateTime.UtcNow - startTime;
                        request.SetResult(result);
                        
                        _logger?.LogDebug("Processed command {RequestId} in {ProcessingTime}ms", 
                            request.RequestId, processingTime.TotalMilliseconds);
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, "Command processing failed for {RequestId}", request.RequestId);
                        request.SetException(ex);
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
            var request = new DelegateCommandRequest<TCommand, TResult>(command);
            
            // 发送到命令处理器
            if (!_commandProcessor.Post(request))
            {
                throw new InvalidOperationException("Unable to queue command for processing - system may be overloaded");
            }
            
            try
            {
                var result = await request.ExecuteAsync(ct);
                return result;
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                _logger?.LogWarning("Command {RequestId} was cancelled", request.RequestId);
                throw;
            }
        }

        private async Task<object> ProcessCommandPipeline(DelegateCommandRequest request)
        {
            // 获取缓存的处理器委托
            var handlerDelegate = GetCachedHandler(request.CommandType);
            
            // 执行处理器
            var result = await handlerDelegate(request.Command, CancellationToken.None);
            return result;
        }

        private Func<object, CancellationToken, Task<object>> GetCachedHandler(Type commandType)
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
                
                // 创建泛型委托
                var handlerDelegateType = typeof(Func<,>).MakeGenericType(commandType, typeof(Task<>).MakeGenericType(resultType));
                
                return new Func<object, CancellationToken, Task<object>>(async (command, ct) =>
                {
                    using var scope = _provider.CreateScope();
                    var handler = scope.ServiceProvider.GetService(handlerType);
                    if (handler == null)
                        throw new InvalidOperationException($"No handler registered for {commandType.Name}");
                    
                    // 使用反射调用HandleAsync方法
                    var handleMethod = handlerType.GetMethod("HandleAsync");
                    if (handleMethod == null)
                        throw new InvalidOperationException($"Handler {handlerType.Name} does not have HandleAsync method");
                    
                    var task = (Task)handleMethod.Invoke(handler, new object[] { command, ct });
                    await task;
                    
                    var resultProperty = task.GetType().GetProperty("Result");
                    return resultProperty?.GetValue(task);
                });
            });
        }

        // 监控和统计方法
        public DelegateBasedCommandBusMetrics GetMetrics()
        {
            return new DelegateBasedCommandBusMetrics
            {
                MaxConcurrency = _maxConcurrency,
                InputQueueSize = _commandProcessor.InputCount,
                CachedHandlers = _handlerCache.Count
            };
        }

        public void ClearCache()
        {
            _handlerCache.Clear();
            _pipelineCache.Clear();
        }

        public void Dispose()
        {
            _commandProcessor?.Complete();
        }
    }

    /// <summary>
    /// 基于委托的命令请求基类
    /// </summary>
    public abstract class DelegateCommandRequest
    {
        public string RequestId { get; }
        public DateTime CreatedAt { get; }
        public Type CommandType { get; }
        public object Command { get; }
        public TaskCompletionSource<object> TaskCompletionSource { get; }

        protected DelegateCommandRequest(object command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            Command = command;
            CommandType = command.GetType();
            CreatedAt = DateTime.UtcNow;
            RequestId = $"{CreatedAt.Ticks}_{Guid.NewGuid():N}";
            TaskCompletionSource = new TaskCompletionSource<object>();
        }

        public async Task<object> ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                var result = await TaskCompletionSource.Task.WaitAsync(cancellationToken);
                return result;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
        }

        public void SetResult(object result)
        {
            TaskCompletionSource.SetResult(result);
        }

        public void SetException(Exception exception)
        {
            TaskCompletionSource.SetException(exception);
        }
    }

    /// <summary>
    /// 强类型的委托命令请求
    /// </summary>
    public class DelegateCommandRequest<TCommand, TResult> : DelegateCommandRequest
        where TCommand : ICommand<TResult>
    {
        public new TCommand Command { get; }

        public DelegateCommandRequest(TCommand command) : base(command)
        {
            Command = command;
        }

        public new async Task<TResult> ExecuteAsync(CancellationToken cancellationToken)
        {
            var result = await base.ExecuteAsync(cancellationToken);
            return (TResult)result;
        }
    }

    /// <summary>
    /// 基于委托的CommandBus统计信息
    /// </summary>
    public class DelegateBasedCommandBusMetrics
    {
        public int MaxConcurrency { get; set; }
        public int InputQueueSize { get; set; }
        public int CachedHandlers { get; set; }
    }
}
