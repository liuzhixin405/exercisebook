using Framework.Core.Abstractions.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using FrameworkMiddleware = Framework.Core.Abstractions.Middleware.IMiddleware;

namespace Framework.Infrastructure.Decorators;

/// <summary>
/// 性能监控中间件 - 装饰器模式
/// 为中间件添加性能监控功能
/// </summary>
public class PerformanceMonitoringMiddleware : FrameworkMiddleware
{
    private readonly IPerformanceMonitor _performanceMonitor;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="performanceMonitor">性能监控器</param>
    public PerformanceMonitoringMiddleware(IPerformanceMonitor performanceMonitor)
    {
        _performanceMonitor = performanceMonitor ?? throw new ArgumentNullException(nameof(performanceMonitor));
    }

    /// <inheritdoc />
    public string Name => "PerformanceMonitoring";

    /// <inheritdoc />
    public int Priority => 1; // 高优先级，最先执行

    /// <inheritdoc />
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var operationName = $"{context.Request.Method} {context.Request.Path}";
        
        using var monitoring = _performanceMonitor.StartMonitoring(operationName);
        var startTime = DateTime.UtcNow;

        try
        {
            await next(context);
        }
        finally
        {
            var duration = DateTime.UtcNow - startTime;
            _performanceMonitor.RecordMetric(operationName, duration);
        }
    }

    /// <inheritdoc />
    public bool ShouldExecute(HttpContext context)
    {
        return true; // 总是执行性能监控
    }
}

/// <summary>
/// 性能监控上下文
/// </summary>
internal class PerformanceMonitoringContext : IDisposable
{
    private readonly IPerformanceMonitor _performanceMonitor;
    private readonly string _operationName;
    private readonly DateTime _startTime;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="performanceMonitor">性能监控器</param>
    /// <param name="operationName">操作名称</param>
    public PerformanceMonitoringContext(IPerformanceMonitor performanceMonitor, string operationName)
    {
        _performanceMonitor = performanceMonitor;
        _operationName = operationName;
        _startTime = DateTime.UtcNow;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        var duration = DateTime.UtcNow - _startTime;
        _performanceMonitor.RecordMetric(_operationName, duration);
    }
}
