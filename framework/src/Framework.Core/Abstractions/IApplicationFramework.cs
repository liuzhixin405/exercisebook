using Framework.Core.Abstractions.Commands;
using Framework.Core.Abstractions.Configuration;
using Framework.Core.Abstractions.Events;
using Framework.Core.Abstractions.Middleware;
using Framework.Core.Abstractions.Proxies;
using Framework.Core.Abstractions.States;
using Framework.Core.Abstractions.Strategies;
using Framework.Core.Abstractions.Visitors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.Design;

namespace Framework.Core.Abstractions;

/// <summary>
/// 应用程序框架主接口 - 外观模式
/// 为复杂的子系统提供统一的接口
/// </summary>
public interface IApplicationFramework
{
    /// <summary>
    /// 服务容器
    /// </summary>
    Framework.Core.Abstractions.Container.IServiceContainer ServiceContainer { get; }

    /// <summary>
    /// 配置构建器
    /// </summary>
    Framework.Core.Abstractions.Configuration.IConfigurationBuilder ConfigurationBuilder { get; }

    /// <summary>
    /// 中间件管道
    /// </summary>
    Framework.Core.Abstractions.Middleware.IMiddlewarePipeline MiddlewarePipeline { get; }

    /// <summary>
    /// 事件总线
    /// </summary>
    Framework.Core.Abstractions.Events.IEventBus EventBus { get; }

    /// <summary>
    /// 命令总线
    /// </summary>
    Framework.Core.Abstractions.Commands.ICommandBus CommandBus { get; }

    /// <summary>
    /// 状态管理器
    /// </summary>
    Framework.Core.Abstractions.States.IStateManager StateManager { get; }

    /// <summary>
    /// 策略上下文
    /// </summary>
    Framework.Core.Abstractions.Strategies.IStrategyContext StrategyContext { get; }

    /// <summary>
    /// 代理工厂
    /// </summary>
    Framework.Core.Abstractions.Proxies.IProxyFactory ProxyFactory { get; }

    /// <summary>
    /// 访问者注册器
    /// </summary>
    Framework.Core.Abstractions.Visitors.IVisitorRegistry VisitorRegistry { get; }

    /// <summary>
    /// 初始化框架
    /// </summary>
    /// <param name="configureServices">服务配置委托</param>
    /// <returns>框架实例</returns>
    Framework.Core.Abstractions.IApplicationFramework ConfigureServices(Action<Framework.Core.Abstractions.Container.IServiceContainer> configureServices);

    /// <summary>
    /// 配置中间件
    /// </summary>
    /// <param name="configureMiddleware">中间件配置委托</param>
    /// <returns>框架实例</returns>
    Framework.Core.Abstractions.IApplicationFramework ConfigureMiddleware(Action<Framework.Core.Abstractions.Middleware.IMiddlewarePipeline> configureMiddleware);

    /// <summary>
    /// 构建应用程序
    /// </summary>
    /// <returns>主机构建器</returns>
    IHostBuilder Build();

    /// <summary>
    /// 运行应用程序
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    Task RunAsync(CancellationToken cancellationToken = default);
}
