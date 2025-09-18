using Microsoft.Extensions.DependencyInjection;

namespace Framework.Core.Abstractions.Container;

/// <summary>
/// 服务容器接口 - 单例模式
/// 提供依赖注入容器的抽象
/// </summary>
public interface IServiceContainer
{
    /// <summary>
    /// 注册单例服务
    /// </summary>
    /// <typeparam name="TService">服务类型</typeparam>
    /// <typeparam name="TImplementation">实现类型</typeparam>
    /// <returns>服务容器</returns>
    IServiceContainer RegisterSingleton<TService, TImplementation>()
        where TService : class
        where TImplementation : class, TService;

    /// <summary>
    /// 注册单例服务
    /// </summary>
    /// <typeparam name="TService">服务类型</typeparam>
    /// <param name="implementationFactory">实现工厂</param>
    /// <returns>服务容器</returns>
    IServiceContainer RegisterSingleton<TService>(Func<IServiceProvider, TService> implementationFactory)
        where TService : class;

    /// <summary>
    /// 注册单例服务实例
    /// </summary>
    /// <typeparam name="TService">服务类型</typeparam>
    /// <param name="instance">服务实例</param>
    /// <returns>服务容器</returns>
    IServiceContainer RegisterSingleton<TService>(TService instance)
        where TService : class;

    /// <summary>
    /// 注册作用域服务
    /// </summary>
    /// <typeparam name="TService">服务类型</typeparam>
    /// <typeparam name="TImplementation">实现类型</typeparam>
    /// <returns>服务容器</returns>
    IServiceContainer RegisterScoped<TService, TImplementation>()
        where TService : class
        where TImplementation : class, TService;

    /// <summary>
    /// 注册瞬态服务
    /// </summary>
    /// <typeparam name="TService">服务类型</typeparam>
    /// <typeparam name="TImplementation">实现类型</typeparam>
    /// <returns>服务容器</returns>
    IServiceContainer RegisterTransient<TService, TImplementation>()
        where TService : class
        where TImplementation : class, TService;

    /// <summary>
    /// 注册服务工厂
    /// </summary>
    /// <typeparam name="TService">服务类型</typeparam>
    /// <param name="factory">服务工厂</param>
    /// <returns>服务容器</returns>
    IServiceContainer RegisterFactory<TService>(IServiceFactory<TService> factory)
        where TService : class;

    /// <summary>
    /// 获取服务
    /// </summary>
    /// <typeparam name="TService">服务类型</typeparam>
    /// <returns>服务实例</returns>
    TService? GetService<TService>() where TService : class;

    /// <summary>
    /// 获取必需服务
    /// </summary>
    /// <typeparam name="TService">服务类型</typeparam>
    /// <returns>服务实例</returns>
    TService GetRequiredService<TService>() where TService : class;

    /// <summary>
    /// 创建作用域
    /// </summary>
    /// <returns>作用域</returns>
    IServiceScope CreateScope();

    /// <summary>
    /// 构建服务提供者
    /// </summary>
    /// <returns>服务提供者</returns>
    IServiceProvider BuildServiceProvider();
}
