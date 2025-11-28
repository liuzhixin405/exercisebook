using Framework.Core.Abstractions.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Framework.Infrastructure.Middleware;

/// <summary>
/// 中间件管道实现 - 责任链模式
/// 提供中间件链式处理的实现
/// </summary>
public class MiddlewarePipeline : IMiddlewarePipeline
{
    private readonly List<IFrameworkMiddleware> _middlewares;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="serviceProvider">服务提供者</param>
    public MiddlewarePipeline(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _middlewares = new List<IFrameworkMiddleware>();
    }

    /// <inheritdoc />
    public IMiddlewarePipeline Use(IFrameworkMiddleware middleware)
    {
        if (middleware == null)
            throw new ArgumentNullException(nameof(middleware));

        _middlewares.Add(middleware);
        return this;
    }

    /// <inheritdoc />
    public IMiddlewarePipeline Use<TMiddleware>() where TMiddleware : class, IFrameworkMiddleware
    {
        var middleware = _serviceProvider.GetService<TMiddleware>();
        if (middleware == null)
        {
            throw new InvalidOperationException($"Middleware of type {typeof(TMiddleware)} is not registered.");
        }

        return Use(middleware);
    }

    /// <inheritdoc />
    public IMiddlewarePipeline Use(Func<HttpContext, Func<Task>, Task> middleware)
    {
        if (middleware == null)
            throw new ArgumentNullException(nameof(middleware));

        var wrapper = new DelegateMiddleware(middleware);
        return Use(wrapper);
    }

    /// <inheritdoc />
    public IMiddlewarePipeline Use(IFrameworkMiddleware middleware, params object[] args)
    {
        if (middleware == null)
            throw new ArgumentNullException(nameof(middleware));

        // 这里可以实现带参数的中间件逻辑
        // 暂时直接添加中间件
        return Use(middleware);
    }

    /// <inheritdoc />
    public RequestDelegate Build()
    {
        // 按优先级排序中间件
        var sortedMiddlewares = _middlewares.OrderBy(m => m.Priority).ToList();

        RequestDelegate pipeline = async context =>
        {
            // 如果没有中间件，直接返回
            if (!sortedMiddlewares.Any())
            {
                return;
            }

            // 构建责任链
            var chain = BuildChain(sortedMiddlewares, 0);
            await chain(context);
        };

        return pipeline;
    }

    /// <inheritdoc />
    public IMiddlewarePipeline Clear()
    {
        _middlewares.Clear();
        return this;
    }

    /// <summary>
    /// 构建责任链
    /// </summary>
    /// <param name="middlewares">中间件列表</param>
    /// <param name="index">当前索引</param>
    /// <returns>请求委托</returns>
    private RequestDelegate BuildChain(List<IFrameworkMiddleware> middlewares, int index)
    {
        if (index >= middlewares.Count)
        {
            return _ => Task.CompletedTask;
        }

        var currentMiddleware = middlewares[index];
        var next = BuildChain(middlewares, index + 1);

        return async context =>
        {
            if (currentMiddleware.ShouldExecute(context))
            {
                await currentMiddleware.InvokeAsync(context, next);
            }
            else
            {
                await next(context);
            }
        };
    }
}

/// <summary>
/// 委托中间件包装器
/// </summary>
internal class DelegateMiddleware : IFrameworkMiddleware
{
    private readonly Func<HttpContext, Func<Task>, Task> _middleware;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="middleware">中间件委托</param>
    public DelegateMiddleware(Func<HttpContext, Func<Task>, Task> middleware)
    {
        _middleware = middleware ?? throw new ArgumentNullException(nameof(middleware));
    }

    /// <inheritdoc />
    public string Name => "Delegate";

    /// <inheritdoc />
    public int Priority => 100;

    /// <inheritdoc />
    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        return _middleware(context, () => next(context));
    }

    /// <inheritdoc />
    public bool ShouldExecute(HttpContext context)
    {
        return true;
    }
}
