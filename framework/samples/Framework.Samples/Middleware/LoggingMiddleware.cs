using Framework.Core.Abstractions.Middleware;
using Microsoft.AspNetCore.Http;

namespace Framework.Samples.Middleware;

/// <summary>
/// 日志中间件
/// </summary>
public class LoggingMiddleware : IMiddleware
{
    /// <inheritdoc />
    public string Name => "LoggingMiddleware";

    /// <inheritdoc />
    public int Priority => 100;

    /// <inheritdoc />
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var startTime = DateTime.UtcNow;
        var requestPath = context.Request.Path;
        var requestMethod = context.Request.Method;

        Console.WriteLine($"[{startTime:HH:mm:ss.fff}] 请求开始: {requestMethod} {requestPath}");

        try
        {
            await next(context);
        }
        finally
        {
            var endTime = DateTime.UtcNow;
            var duration = endTime - startTime;
            var statusCode = context.Response.StatusCode;

            Console.WriteLine($"[{endTime:HH:mm:ss.fff}] 请求完成: {requestMethod} {requestPath} - {statusCode} ({duration.TotalMilliseconds}ms)");
        }
    }

    /// <inheritdoc />
    public bool ShouldExecute(HttpContext context)
    {
        return true; // 总是执行日志记录
    }
}
