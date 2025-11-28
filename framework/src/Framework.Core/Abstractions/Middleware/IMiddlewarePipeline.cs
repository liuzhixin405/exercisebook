using Microsoft.AspNetCore.Http;

namespace Framework.Core.Abstractions.Middleware;

/// <summary>
/// 中间件管道接口 - 责任链模式
/// 提供中间件链式处理的抽象
/// </summary>
public interface IMiddlewarePipeline
{
    /// <summary>
    /// 添加中间件
    /// </summary>
    /// <param name="middleware">中间件</param>
    /// <returns>中间件管道</returns>
    IMiddlewarePipeline Use(IFrameworkMiddleware middleware);

    /// <summary>
    /// 添加中间件（泛型）
    /// </summary>
    /// <typeparam name="TMiddleware">中间件类型</typeparam>
    /// <returns>中间件管道</returns>
    IMiddlewarePipeline Use<TMiddleware>() where TMiddleware : class, IFrameworkMiddleware;

    /// <summary>
    /// 添加中间件（委托）
    /// </summary>
    /// <param name="middleware">中间件委托</param>
    /// <returns>中间件管道</returns>
    IMiddlewarePipeline Use(Func<HttpContext, Func<Task>, Task> middleware);

    /// <summary>
    /// 添加中间件（带参数）
    /// </summary>
    /// <param name="middleware">中间件</param>
    /// <param name="args">参数</param>
    /// <returns>中间件管道</returns>
    IMiddlewarePipeline Use(IFrameworkMiddleware middleware, params object[] args);

    /// <summary>
    /// 构建中间件管道
    /// </summary>
    /// <returns>请求委托</returns>
    RequestDelegate Build();

    /// <summary>
    /// 清空中间件
    /// </summary>
    /// <returns>中间件管道</returns>
    IMiddlewarePipeline Clear();
}
