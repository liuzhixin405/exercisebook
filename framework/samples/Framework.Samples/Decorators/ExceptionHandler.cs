using Framework.Infrastructure.Decorators;

namespace Framework.Samples.Decorators;

/// <summary>
/// 异常处理器实现 - 装饰器模式示例
/// </summary>
public class ExceptionHandler : IExceptionHandler
{
    public Task<bool> HandleExceptionAsync(Exception exception, object? context = null)
    {
        Console.WriteLine($"[异常处理] 类型: {exception.GetType().Name}");
        Console.WriteLine($"[异常处理] 消息: {exception.Message}");
        if (context != null)
        {
            Console.WriteLine($"[异常处理] 上下文: {context}");
        }
        
        // 根据异常类型决定是否已处理
        return Task.FromResult(exception is not SystemException);
    }
}
