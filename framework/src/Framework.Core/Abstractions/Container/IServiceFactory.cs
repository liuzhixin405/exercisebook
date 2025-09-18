namespace Framework.Core.Abstractions.Container;

/// <summary>
/// 服务工厂接口 - 工厂模式
/// 提供创建服务实例的抽象
/// </summary>
/// <typeparam name="TService">服务类型</typeparam>
public interface IServiceFactory<out TService> where TService : class
{
    /// <summary>
    /// 创建服务实例
    /// </summary>
    /// <param name="serviceProvider">服务提供者</param>
    /// <returns>服务实例</returns>
    TService Create(IServiceProvider serviceProvider);

    /// <summary>
    /// 创建服务实例（带参数）
    /// </summary>
    /// <param name="serviceProvider">服务提供者</param>
    /// <param name="parameters">参数</param>
    /// <returns>服务实例</returns>
    TService Create(IServiceProvider serviceProvider, params object[] parameters);
}
