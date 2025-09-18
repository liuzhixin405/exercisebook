using Framework.Core.Abstractions.Proxies;

namespace Framework.Samples.Interceptors;

/// <summary>
/// 日志拦截器
/// </summary>
public class LoggingInterceptor : IInterceptor
{
    /// <inheritdoc />
    public string Name => "LoggingInterceptor";

    /// <inheritdoc />
    public int Priority => 100;

    /// <inheritdoc />
    public async Task InterceptAsync(IInvocation invocation)
    {
        var startTime = DateTime.UtcNow;
        var methodName = $"{invocation.Target.GetType().Name}.{invocation.Method.Name}";
        
        Console.WriteLine($"[{startTime:HH:mm:ss.fff}] 方法调用开始: {methodName}");
        Console.WriteLine($"参数: {string.Join(", ", invocation.Arguments)}");

        try
        {
            // 继续执行
            await invocation.ProceedAsync();
            
            var endTime = DateTime.UtcNow;
            var duration = endTime - startTime;
            
            Console.WriteLine($"[{endTime:HH:mm:ss.fff}] 方法调用完成: {methodName} - 返回值: {invocation.ReturnValue} ({duration.TotalMilliseconds}ms)");
        }
        catch (Exception ex)
        {
            var endTime = DateTime.UtcNow;
            var duration = endTime - startTime;
            
            Console.WriteLine($"[{endTime:HH:mm:ss.fff}] 方法调用异常: {methodName} - 异常: {ex.Message} ({duration.TotalMilliseconds}ms)");
            throw;
        }
    }

    /// <inheritdoc />
    public bool ShouldIntercept(IInvocation invocation)
    {
        // 拦截所有方法调用
        return true;
    }
}
