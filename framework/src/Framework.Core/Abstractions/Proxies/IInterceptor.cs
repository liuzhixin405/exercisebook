namespace Framework.Core.Abstractions.Proxies;

/// <summary>
/// 拦截器接口 - 代理模式
/// 提供方法拦截的抽象
/// </summary>
public interface IInterceptor
{
    /// <summary>
    /// 拦截器名称
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 拦截器优先级（数字越小优先级越高）
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// 拦截方法调用
    /// </summary>
    /// <param name="invocation">方法调用信息</param>
    /// <returns>任务</returns>
    Task InterceptAsync(IInvocation invocation);

    /// <summary>
    /// 是否应该拦截此方法
    /// </summary>
    /// <param name="invocation">方法调用信息</param>
    /// <returns>是否应该拦截</returns>
    bool ShouldIntercept(IInvocation invocation);
}
