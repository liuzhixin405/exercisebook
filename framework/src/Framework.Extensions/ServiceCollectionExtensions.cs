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
using Framework.Infrastructure;
using Framework.Infrastructure.Commands;
using Framework.Infrastructure.Configuration;
using Framework.Infrastructure.Container;
using Framework.Infrastructure.Events;
using Framework.Infrastructure.Mediators;
using Framework.Infrastructure.Memento;
using Framework.Infrastructure.Middleware;
using Framework.Infrastructure.Proxies;
using Framework.Infrastructure.States;
using Framework.Infrastructure.Strategies;
using Framework.Infrastructure.Visitors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Framework.Extensions;

/// <summary>
/// 服务集合扩展方法
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加框架服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddFramework(this IServiceCollection services)
    {
        // 注册核心服务
        services.AddSingleton<IServiceContainer, ServiceContainer>();
        services.AddSingleton<IConfigurationBuilder>(sp => (IConfigurationBuilder)new Framework.Infrastructure.Configuration.ConfigurationBuilder());
        services.AddSingleton<IMiddlewarePipeline, MiddlewarePipeline>();
        services.AddSingleton<IEventBus, EventBus>();
        services.AddSingleton<ICommandBus, CommandBus>();
        services.AddSingleton<IStateManager, StateManager>();
        services.AddSingleton<IStrategyContext, StrategyContext>();
        services.AddSingleton<IProxyFactory, ProxyFactory>();
        services.AddSingleton<IVisitorRegistry, VisitorRegistry>();

        // 注册高级模式服务
        services.AddSingleton<IMediator, Mediator>();
        services.AddSingleton<IMementoManager, MementoManager>();

        // 注册框架主入口
        services.AddSingleton<IApplicationFramework, ApplicationFramework>();

        // Hosted service to register discovered command handlers into ICommandBus at startup
        services.AddHostedService<CommandHandlerRegistrar>();

        return services;
    }

    /// <summary>
    /// 添加框架服务（带配置）
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configureFramework">框架配置委托</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddFramework(this IServiceCollection services, Action<IApplicationFramework> configureFramework)
    {
        services.AddFramework();

        // 配置框架
        services.Configure<FrameworkOptions>(options =>
        {
            options.ConfigureFramework = configureFramework;
        });

        return services;
    }

    /// <summary>
    /// 添加中间件
    /// </summary>
    /// <typeparam name="TMiddleware">中间件类型</typeparam>
    /// <param name="services">服务集合</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddMiddleware<TMiddleware>(this IServiceCollection services)
        where TMiddleware : class
    {
        services.AddTransient<TMiddleware>();
        return services;
    }

    /// <summary>
    /// 添加事件处理器
    /// </summary>
    /// <typeparam name="TEvent">事件类型</typeparam>
    /// <typeparam name="THandler">处理器类型</typeparam>
    /// <param name="services">服务集合</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddEventHandler<TEvent, THandler>(this IServiceCollection services)
        where TEvent : class
        where THandler : class, IEventHandler<TEvent>
    {
        services.AddTransient<IEventHandler<TEvent>, THandler>();
        return services;
    }

    /// <summary>
    /// 添加命令处理器
    /// </summary>
    /// <typeparam name="TCommand">命令类型</typeparam>
    /// <typeparam name="THandler">处理器类型</typeparam>
    /// <param name="services">服务集合</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddCommandHandler<TCommand, THandler>(this IServiceCollection services)
        where TCommand : class, ICommand
        where THandler : class, ICommandHandler<TCommand>
    {
        services.AddTransient<ICommandHandler<TCommand>, THandler>();
        return services;
    }

    /// <summary>
    /// 添加命令处理器（带结果）
    /// </summary>
    /// <typeparam name="TCommand">命令类型</typeparam>
    /// <typeparam name="TResult">结果类型</typeparam>
    /// <typeparam name="THandler">处理器类型</typeparam>
    /// <param name="services">服务集合</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddCommandHandler<TCommand, TResult, THandler>(this IServiceCollection services)
        where TCommand : class, ICommand<TResult>
        where THandler : class, ICommandHandler<TCommand, TResult>
    {
        services.AddTransient<ICommandHandler<TCommand, TResult>, THandler>();
        return services;
    }

    /// <summary>
    /// 添加策略
    /// </summary>
    /// <typeparam name="TStrategy">策略类型</typeparam>
    /// <param name="services">服务集合</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddStrategy<TStrategy>(this IServiceCollection services)
        where TStrategy : class, IStrategy
    {
        services.AddTransient<TStrategy>();
        return services;
    }

    /// <summary>
    /// 添加状态
    /// </summary>
    /// <typeparam name="TState">状态类型</typeparam>
    /// <param name="services">服务集合</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddState<TState>(this IServiceCollection services)
        where TState : class, IState
    {
        services.AddTransient<TState>();
        return services;
    }

    /// <summary>
    /// 添加访问者
    /// </summary>
    /// <typeparam name="TVisitable">可访问类型</typeparam>
    /// <typeparam name="TVisitor">访问者类型</typeparam>
    /// <param name="services">服务集合</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddVisitor<TVisitable, TVisitor>(this IServiceCollection services)
        where TVisitable : class
        where TVisitor : class
    {
        // Register the concrete visitor type
        services.AddTransient<TVisitor>();

        // Try to also register as IVisitor<TVisitable> if the type implements it
        try
        {
            var visitorInterface = typeof(TVisitor).GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IVisitor<>) && i.GetGenericArguments()[0] == typeof(TVisitable));
            if (visitorInterface != null)
            {
                services.AddTransient(visitorInterface, typeof(TVisitor));
            }
        }
        catch
        {
            // ignore reflection registration failures
        }

        return services;
    }

    /// <summary>
    /// 添加拦截器
    /// </summary>
    /// <typeparam name="TInterceptor">拦截器类型</typeparam>
    /// <param name="services">服务集合</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddInterceptor<TInterceptor>(this IServiceCollection services)
        where TInterceptor : class, IInterceptor
    {
        services.AddTransient<TInterceptor>();
        return services;
    }
}

/// <summary>
/// 框架选项
/// </summary>
public class FrameworkOptions
{
    /// <summary>
    /// 框架配置委托
    /// </summary>
    public Action<IApplicationFramework>? ConfigureFramework { get; set; }
}
