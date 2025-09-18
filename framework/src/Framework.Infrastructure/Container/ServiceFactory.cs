using Framework.Core.Abstractions.Container;

namespace Framework.Infrastructure.Container;

/// <summary>
/// 服务工厂实现 - 工厂模式
/// 提供创建服务实例的实现
/// </summary>
/// <typeparam name="TService">服务类型</typeparam>
public class ServiceFactory<TService> : IServiceFactory<TService> where TService : class
{
    private readonly Func<IServiceProvider, TService> _factory;
    private readonly Func<IServiceProvider, object[], TService>? _parameterizedFactory;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="factory">服务工厂委托</param>
    public ServiceFactory(Func<IServiceProvider, TService> factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    /// <summary>
    /// 构造函数（带参数）
    /// </summary>
    /// <param name="factory">服务工厂委托</param>
    /// <param name="parameterizedFactory">带参数的服务工厂委托</param>
    public ServiceFactory(
        Func<IServiceProvider, TService> factory,
        Func<IServiceProvider, object[], TService> parameterizedFactory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _parameterizedFactory = parameterizedFactory ?? throw new ArgumentNullException(nameof(parameterizedFactory));
    }

    /// <inheritdoc />
    public TService Create(IServiceProvider serviceProvider)
    {
        return _factory(serviceProvider);
    }

    /// <inheritdoc />
    public TService Create(IServiceProvider serviceProvider, params object[] parameters)
    {
        if (_parameterizedFactory != null)
        {
            return _parameterizedFactory(serviceProvider, parameters);
        }

        // 如果没有参数化工厂，尝试使用反射创建实例
        var constructor = typeof(TService).GetConstructors()
            .FirstOrDefault(c => c.GetParameters().Length == parameters.Length);

        if (constructor != null)
        {
            return (TService)Activator.CreateInstance(typeof(TService), parameters)!;
        }

        // 回退到无参数工厂
        return _factory(serviceProvider);
    }
}
