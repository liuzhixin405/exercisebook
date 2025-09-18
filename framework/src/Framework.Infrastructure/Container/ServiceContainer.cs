using Framework.Core.Abstractions.Container;
using Microsoft.Extensions.DependencyInjection;

namespace Framework.Infrastructure.Container;

/// <summary>
/// 服务容器实现 - 单例模式
/// 提供依赖注入容器的实现
/// </summary>
public class ServiceContainer : IServiceContainer
{
    private readonly IServiceCollection _services;
    private IServiceProvider? _serviceProvider;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="services">服务集合</param>
    public ServiceContainer(IServiceCollection? services = null)
    {
        _services = services ?? new ServiceCollection();
    }

    /// <inheritdoc />
    public IServiceContainer RegisterSingleton<TService, TImplementation>()
        where TService : class
        where TImplementation : class, TService
    {
        _services.AddSingleton<TService, TImplementation>();
        return this;
    }

    /// <inheritdoc />
    public IServiceContainer RegisterSingleton<TService>(Func<IServiceProvider, TService> implementationFactory)
        where TService : class
    {
        _services.AddSingleton(implementationFactory);
        return this;
    }

    /// <inheritdoc />
    public IServiceContainer RegisterSingleton<TService>(TService instance)
        where TService : class
    {
        _services.AddSingleton(instance);
        return this;
    }

    /// <inheritdoc />
    public IServiceContainer RegisterScoped<TService, TImplementation>()
        where TService : class
        where TImplementation : class, TService
    {
        _services.AddScoped<TService, TImplementation>();
        return this;
    }

    /// <inheritdoc />
    public IServiceContainer RegisterTransient<TService, TImplementation>()
        where TService : class
        where TImplementation : class, TService
    {
        _services.AddTransient<TService, TImplementation>();
        return this;
    }

    /// <inheritdoc />
    public IServiceContainer RegisterFactory<TService>(IServiceFactory<TService> factory)
        where TService : class
    {
        _services.AddSingleton(factory);
        _services.AddTransient<TService>(provider => factory.Create(provider));
        return this;
    }

    /// <inheritdoc />
    public TService? GetService<TService>() where TService : class
    {
        EnsureServiceProvider();
        return _serviceProvider?.GetService<TService>();
    }

    /// <inheritdoc />
    public TService GetRequiredService<TService>() where TService : class
    {
        EnsureServiceProvider();
        return _serviceProvider?.GetRequiredService<TService>() 
            ?? throw new InvalidOperationException($"Service of type {typeof(TService)} is not registered.");
    }

    /// <inheritdoc />
    public IServiceScope CreateScope()
    {
        EnsureServiceProvider();
        return _serviceProvider?.CreateScope() 
            ?? throw new InvalidOperationException("Service provider is not available.");
    }

    /// <inheritdoc />
    public IServiceProvider BuildServiceProvider()
    {
        _serviceProvider = _services.BuildServiceProvider();
        return _serviceProvider;
    }

    /// <summary>
    /// 确保服务提供者已构建
    /// </summary>
    private void EnsureServiceProvider()
    {
        _serviceProvider ??= _services.BuildServiceProvider();
    }
}
