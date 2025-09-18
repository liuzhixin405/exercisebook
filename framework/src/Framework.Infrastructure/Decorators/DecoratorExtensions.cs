using Framework.Core.Abstractions.Container;
using Framework.Core.Abstractions.Events;
using Framework.Core.Abstractions.Middleware;
using Framework.Core.Abstractions.Proxies;
using Microsoft.Extensions.Logging;

namespace Framework.Infrastructure.Decorators;

/// <summary>
/// 装饰器扩展方法 - 装饰器模式
/// 提供功能增强的装饰器
/// </summary>
public static class DecoratorExtensions
{
    /// <summary>
    /// 为服务添加日志装饰器
    /// </summary>
    /// <typeparam name="TService">服务类型</typeparam>
    /// <param name="container">服务容器</param>
    /// <param name="logger">日志记录器</param>
    /// <returns>服务容器</returns>
    public static IServiceContainer WithLogging<TService>(this IServiceContainer container, ILogger<TService> logger)
        where TService : class
    {
        container.RegisterSingleton<ILogger<TService>>(logger);
        return container;
    }

    /// <summary>
    /// 为服务添加缓存装饰器
    /// </summary>
    /// <typeparam name="TService">服务类型</typeparam>
    /// <param name="container">服务容器</param>
    /// <param name="cacheProvider">缓存提供者</param>
    /// <returns>服务容器</returns>
    public static IServiceContainer WithCaching<TService>(this IServiceContainer container, ICacheProvider cacheProvider)
        where TService : class
    {
        container.RegisterSingleton<ICacheProvider>(cacheProvider);
        return container;
    }

    /// <summary>
    /// 为服务添加重试装饰器
    /// </summary>
    /// <typeparam name="TService">服务类型</typeparam>
    /// <param name="container">服务容器</param>
    /// <param name="maxRetries">最大重试次数</param>
    /// <param name="delay">重试延迟</param>
    /// <returns>服务容器</returns>
    public static IServiceContainer WithRetry<TService>(this IServiceContainer container, int maxRetries = 3, TimeSpan? delay = null)
        where TService : class
    {
        var retryOptions = new RetryOptions(maxRetries, delay ?? TimeSpan.FromSeconds(1));
        container.RegisterSingleton(retryOptions);
        return container;
    }

    /// <summary>
    /// 为中间件添加性能监控装饰器
    /// </summary>
    /// <param name="pipeline">中间件管道</param>
    /// <param name="performanceMonitor">性能监控器</param>
    /// <returns>中间件管道</returns>
    public static IMiddlewarePipeline WithPerformanceMonitoring(this IMiddlewarePipeline pipeline, IPerformanceMonitor performanceMonitor)
    {
        var decorator = new PerformanceMonitoringMiddleware(performanceMonitor);
        return pipeline.Use(decorator);
    }

    /// <summary>
    /// 为中间件添加异常处理装饰器
    /// </summary>
    /// <param name="pipeline">中间件管道</param>
    /// <param name="exceptionHandler">异常处理器</param>
    /// <returns>中间件管道</returns>
    public static IMiddlewarePipeline WithExceptionHandling(this IMiddlewarePipeline pipeline, IExceptionHandler exceptionHandler)
    {
        var decorator = new ExceptionHandlingMiddleware(exceptionHandler);
        return pipeline.Use(decorator);
    }

    /// <summary>
    /// 为事件总线添加审计装饰器
    /// </summary>
    /// <param name="eventBus">事件总线</param>
    /// <param name="auditLogger">审计日志记录器</param>
    /// <returns>装饰后的事件总线</returns>
    public static IEventBus WithAuditing(this IEventBus eventBus, IAuditLogger auditLogger)
    {
        return new AuditingEventBus(eventBus, auditLogger);
    }

    /// <summary>
    /// 为代理工厂添加拦截器装饰器
    /// </summary>
    /// <param name="proxyFactory">代理工厂</param>
    /// <param name="interceptors">拦截器</param>
    /// <returns>代理工厂</returns>
    public static IProxyFactory WithInterceptors(this IProxyFactory proxyFactory, params IInterceptor[] interceptors)
    {
        foreach (var interceptor in interceptors)
        {
            proxyFactory.RegisterInterceptor(interceptor);
        }
        return proxyFactory;
    }
}

/// <summary>
/// 重试选项
/// </summary>
public class RetryOptions
{
    /// <summary>
    /// 最大重试次数
    /// </summary>
    public int MaxRetries { get; }

    /// <summary>
    /// 重试延迟
    /// </summary>
    public TimeSpan Delay { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="maxRetries">最大重试次数</param>
    /// <param name="delay">重试延迟</param>
    public RetryOptions(int maxRetries, TimeSpan delay)
    {
        MaxRetries = maxRetries;
        Delay = delay;
    }
}

/// <summary>
/// 缓存提供者接口
/// </summary>
public interface ICacheProvider
{
    /// <summary>
    /// 获取缓存值
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="key">键</param>
    /// <returns>缓存值</returns>
    T? Get<T>(string key);

    /// <summary>
    /// 设置缓存值
    /// </summary>
    /// <typeparam name="T">值类型</typeparam>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <param name="expiration">过期时间</param>
    void Set<T>(string key, T value, TimeSpan? expiration = null);

    /// <summary>
    /// 移除缓存值
    /// </summary>
    /// <param name="key">键</param>
    void Remove(string key);
}

/// <summary>
/// 性能监控器接口
/// </summary>
public interface IPerformanceMonitor
{
    /// <summary>
    /// 开始监控
    /// </summary>
    /// <param name="operationName">操作名称</param>
    /// <returns>监控上下文</returns>
    IDisposable StartMonitoring(string operationName);

    /// <summary>
    /// 记录性能指标
    /// </summary>
    /// <param name="operationName">操作名称</param>
    /// <param name="duration">持续时间</param>
    void RecordMetric(string operationName, TimeSpan duration);
}

/// <summary>
/// 异常处理器接口
/// </summary>
public interface IExceptionHandler
{
    /// <summary>
    /// 处理异常
    /// </summary>
    /// <param name="exception">异常</param>
    /// <param name="context">上下文</param>
    /// <returns>是否已处理</returns>
    Task<bool> HandleExceptionAsync(Exception exception, object? context = null);
}

/// <summary>
/// 审计日志记录器接口
/// </summary>
public interface IAuditLogger
{
    /// <summary>
    /// 记录审计日志
    /// </summary>
    /// <param name="operation">操作</param>
    /// <param name="details">详细信息</param>
    Task LogAuditAsync(string operation, object? details = null);
}
