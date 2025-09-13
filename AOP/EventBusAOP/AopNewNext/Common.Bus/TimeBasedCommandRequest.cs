using System;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Bus
{
    /// <summary>
    /// 基于时间戳的命令请求，简化泛型打包的复杂性
    /// </summary>
    public class TimeBasedCommandRequest
    {
        /// <summary>
        /// 基于时间戳的唯一请求标识
        /// </summary>
        public string RequestId { get; }
        
        /// <summary>
        /// 请求创建时间
        /// </summary>
        public DateTime CreatedAt { get; }
        
        /// <summary>
        /// 命令类型
        /// </summary>
        public Type CommandType { get; }
        
        /// <summary>
        /// 结果类型
        /// </summary>
        public Type ResultType { get; }
        
        /// <summary>
        /// 命令对象
        /// </summary>
        public object Command { get; }
        
        /// <summary>
        /// 任务完成源
        /// </summary>
        public TaskCompletionSource<object> TaskCompletionSource { get; }
        
        /// <summary>
        /// 请求超时时间
        /// </summary>
        public TimeSpan? Timeout { get; }
        
        /// <summary>
        /// 请求优先级（基于时间戳，越小优先级越高）
        /// </summary>
        public long Priority => CreatedAt.Ticks;

        public TimeBasedCommandRequest(object command, TimeSpan? timeout = null)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            Command = command;
            CommandType = command.GetType();
            CreatedAt = DateTime.UtcNow;
            RequestId = $"{CreatedAt.Ticks}_{Guid.NewGuid():N}";
            TaskCompletionSource = new TaskCompletionSource<object>();
            Timeout = timeout;

            // 自动推断结果类型
            ResultType = InferResultType(CommandType);
        }

        /// <summary>
        /// 执行命令并返回结果
        /// </summary>
        public async Task<object> ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (Timeout.HasValue)
                {
                    using var timeoutCts = new CancellationTokenSource(Timeout.Value);
                    using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);
                    var result = await TaskCompletionSource.Task.WaitAsync(combinedCts.Token);
                    return result;
                }
                else
                {
                    var result = await TaskCompletionSource.Task.WaitAsync(cancellationToken);
                    return result;
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (OperationCanceledException) when (Timeout.HasValue)
            {
                throw new TimeoutException($"Command request {RequestId} timed out after {Timeout.Value.TotalMilliseconds}ms");
            }
        }

        /// <summary>
        /// 设置执行结果
        /// </summary>
        public void SetResult(object result)
        {
            TaskCompletionSource.SetResult(result);
        }

        /// <summary>
        /// 设置异常
        /// </summary>
        public void SetException(Exception exception)
        {
            TaskCompletionSource.SetException(exception);
        }

        /// <summary>
        /// 从命令类型推断结果类型
        /// </summary>
        private static Type InferResultType(Type commandType)
        {
            // 查找ICommand<TResult>接口
            var commandInterface = commandType.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommand<>));
            
            if (commandInterface != null)
            {
                return commandInterface.GetGenericArguments()[0];
            }

            // 如果没有找到ICommand<TResult>接口，返回object类型
            return typeof(object);
        }

        /// <summary>
        /// 获取时间窗口标识（用于批处理）
        /// </summary>
        public string GetTimeWindowId(TimeSpan windowSize)
        {
            var windowStart = new DateTime(CreatedAt.Ticks / windowSize.Ticks * windowSize.Ticks);
            return windowStart.Ticks.ToString();
        }

        /// <summary>
        /// 检查请求是否在指定时间窗口内
        /// </summary>
        public bool IsInTimeWindow(DateTime windowStart, TimeSpan windowSize)
        {
            var windowEnd = windowStart.Add(windowSize);
            return CreatedAt >= windowStart && CreatedAt < windowEnd;
        }

        public override string ToString()
        {
            return $"TimeBasedCommandRequest[{RequestId}] - {CommandType.Name} at {CreatedAt:yyyy-MM-dd HH:mm:ss.fff}";
        }
    }
}
