using Framework.Core.Abstractions;
using Framework.Core.Abstractions.Commands;
using Framework.Core.Abstractions.Configuration;
using Framework.Core.Abstractions.Container;
using Framework.Core.Abstractions.Events;
using Framework.Core.Abstractions.Middleware;
using Framework.Core.Abstractions.Proxies;
using Framework.Core.Abstractions.States;
using Framework.Core.Abstractions.Strategies;
using Framework.Core.Abstractions.Visitors;
using Framework.Infrastructure.Commands;
using Framework.Infrastructure.Configuration;
using Framework.Infrastructure.Container;
using Framework.Infrastructure.Events;
using Framework.Infrastructure.Middleware;
using Framework.Infrastructure.Proxies;
using Framework.Infrastructure.States;
using Framework.Infrastructure.Strategies;
using Framework.Infrastructure.Visitors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Framework.Infrastructure;

/// <summary>
/// 应用程序框架实现 - 外观模式
/// 为复杂的子系统提供统一的接口
/// </summary>
public class ApplicationFramework : IApplicationFramework
{
    private readonly IServiceCollection _services;
    private IHostBuilder? _hostBuilder;

    /// <summary>
    /// 构造函数
    /// </summary>
    public ApplicationFramework()
    {
        _services = new ServiceCollection();
        InitializeServices();
    }

    /// <inheritdoc />
    public IServiceContainer ServiceContainer { get; private set; } = null!;

    /// <inheritdoc />
    public IConfigurationBuilder ConfigurationBuilder { get; private set; } = null!;

    /// <inheritdoc />
    public IMiddlewarePipeline MiddlewarePipeline { get; private set; } = null!;

    /// <inheritdoc />
    public IEventBus EventBus { get; private set; } = null!;

    /// <inheritdoc />
    public ICommandBus CommandBus { get; private set; } = null!;

    /// <inheritdoc />
    public IStateManager StateManager { get; private set; } = null!;

    /// <inheritdoc />
    public IStrategyContext StrategyContext { get; private set; } = null!;

    /// <inheritdoc />
    public IProxyFactory ProxyFactory { get; private set; } = null!;

    /// <inheritdoc />
    public IVisitorRegistry VisitorRegistry { get; private set; } = null!;

    /// <inheritdoc />
    public IApplicationFramework ConfigureServices(Action<IServiceContainer> configureServices)
    {
        configureServices?.Invoke(ServiceContainer);
        return this;
    }

    /// <inheritdoc />
    public IApplicationFramework ConfigureMiddleware(Action<IMiddlewarePipeline> configureMiddleware)
    {
        configureMiddleware?.Invoke(MiddlewarePipeline);
        return this;
    }

    /// <inheritdoc />
    public IHostBuilder Build()
    {
        // 构建服务提供者
        var serviceProvider = ServiceContainer.BuildServiceProvider();

        // 创建主机构建器
        _hostBuilder = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                // 添加框架服务
                services.AddSingleton(ServiceContainer);
                services.AddSingleton(ConfigurationBuilder);
                services.AddSingleton(MiddlewarePipeline);
                services.AddSingleton(EventBus);
                services.AddSingleton(CommandBus);
                services.AddSingleton(StateManager);
                services.AddSingleton(StrategyContext);
                services.AddSingleton(ProxyFactory);
                services.AddSingleton(VisitorRegistry);

                // 添加用户配置的服务
                foreach (var service in _services)
                {
                    services.Add(service);
                }
            });

        return _hostBuilder;
    }

    /// <inheritdoc />
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        if (_hostBuilder == null)
        {
            throw new InvalidOperationException("Framework must be built before running. Call Build() first.");
        }

        using var host = _hostBuilder.Build();
        await host.RunAsync(cancellationToken);
    }

    /// <summary>
    /// 初始化服务
    /// </summary>
    private void InitializeServices()
    {
        // 创建服务容器
        ServiceContainer = new ServiceContainer(_services);

        // 创建配置构建器
        ConfigurationBuilder = new ConfigurationBuilder();

        // 创建中间件管道
        var tempServiceProvider = _services.BuildServiceProvider();
        MiddlewarePipeline = new MiddlewarePipeline(tempServiceProvider);

        // 创建事件总线
        EventBus = new EventBus(tempServiceProvider);

        // 创建命令总线
        CommandBus = new CommandBus(tempServiceProvider);

        // 创建状态管理器
        StateManager = new StateManager();

        // 创建策略上下文
        StrategyContext = new StrategyContext();

        // 创建代理工厂
        ProxyFactory = new ProxyFactory();

        // 创建访问者注册器
        VisitorRegistry = new VisitorRegistry();

        // 注册核心服务
        RegisterCoreServices();
    }

    /// <summary>
    /// 注册核心服务
    /// </summary>
    private void RegisterCoreServices()
    {
        ServiceContainer
            .RegisterSingleton<IServiceContainer>(ServiceContainer)
            .RegisterSingleton<IConfigurationBuilder>(ConfigurationBuilder)
            .RegisterSingleton<IMiddlewarePipeline>(MiddlewarePipeline)
            .RegisterSingleton<IEventBus>(EventBus)
            .RegisterSingleton<ICommandBus>(CommandBus)
            .RegisterSingleton<IStateManager>(StateManager)
            .RegisterSingleton<IStrategyContext>(StrategyContext)
            .RegisterSingleton<IProxyFactory>(ProxyFactory)
            .RegisterSingleton<IVisitorRegistry>(VisitorRegistry);
    }
}
