namespace Framework.Core.Abstractions.Proxies;

/// <summary>
/// 代理工厂接口 - 代理模式
/// 提供创建代理对象的抽象
/// </summary>
public interface IProxyFactory
{
    /// <summary>
    /// 创建代理
    /// </summary>
    /// <typeparam name="TInterface">接口类型</typeparam>
    /// <param name="target">目标对象</param>
    /// <param name="interceptors">拦截器</param>
    /// <returns>代理对象</returns>
    TInterface CreateProxy<TInterface>(TInterface target, params IInterceptor[] interceptors)
        where TInterface : class;

    /// <summary>
    /// 创建代理（带工厂）
    /// </summary>
    /// <typeparam name="TInterface">接口类型</typeparam>
    /// <param name="factory">目标对象工厂</param>
    /// <param name="interceptors">拦截器</param>
    /// <returns>代理对象</returns>
    TInterface CreateProxy<TInterface>(Func<TInterface> factory, params IInterceptor[] interceptors)
        where TInterface : class;

    /// <summary>
    /// 创建代理（带参数）
    /// </summary>
    /// <typeparam name="TInterface">接口类型</typeparam>
    /// <param name="factory">目标对象工厂</param>
    /// <param name="parameters">参数</param>
    /// <param name="interceptors">拦截器</param>
    /// <returns>代理对象</returns>
    TInterface CreateProxy<TInterface>(Func<object[], TInterface> factory, object[] parameters, params IInterceptor[] interceptors)
        where TInterface : class;

    /// <summary>
    /// 注册拦截器
    /// </summary>
    /// <param name="interceptor">拦截器</param>
    /// <returns>代理工厂</returns>
    IProxyFactory RegisterInterceptor(IInterceptor interceptor);

    /// <summary>
    /// 移除拦截器
    /// </summary>
    /// <param name="interceptor">拦截器</param>
    /// <returns>代理工厂</returns>
    IProxyFactory RemoveInterceptor(IInterceptor interceptor);

    /// <summary>
    /// 清空所有拦截器
    /// </summary>
    /// <returns>代理工厂</returns>
    IProxyFactory ClearInterceptors();
}
