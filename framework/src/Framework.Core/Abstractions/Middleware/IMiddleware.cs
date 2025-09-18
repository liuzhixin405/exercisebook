using Microsoft.AspNetCore.Http;

namespace Framework.Core.Abstractions.Middleware;

/// <summary>
/// 中间件接口 - 责任链模式
/// 提供中间件处理的抽象
/// </summary>
public interface IMiddleware
{
    /// <summary>
    /// 中间件名称
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 中间件优先级（数字越小优先级越高）
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// 处理请求
    /// </summary>
    /// <param name="context">HTTP上下文</param>
    /// <param name="next">下一个中间件</param>
    /// <returns>任务</returns>
    Task InvokeAsync(HttpContext context, RequestDelegate next);

    /// <summary>
    /// 是否应该执行此中间件
    /// </summary>
    /// <param name="context">HTTP上下文</param>
    /// <returns>是否应该执行</returns>
    bool ShouldExecute(HttpContext context);
}
