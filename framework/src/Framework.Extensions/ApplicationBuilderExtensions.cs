using Framework.Core.Abstractions;
using Framework.Core.Abstractions.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Framework.Extensions;

/// <summary>
/// 应用程序构建器扩展方法
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// 使用框架
    /// </summary>
    /// <param name="app">应用程序构建器</param>
    /// <returns>应用程序构建器</returns>
    public static IApplicationBuilder UseFramework(this IApplicationBuilder app)
    {
        var framework = app.ApplicationServices.GetRequiredService<IApplicationFramework>();
        var pipeline = framework.MiddlewarePipeline;
        
        app.Use(async (context, next) =>
        {
            await pipeline.Build()(context);
        });

        return app;
    }

    /// <summary>
    /// 使用框架中间件
    /// </summary>
    /// <param name="app">应用程序构建器</param>
    /// <param name="configureMiddleware">中间件配置委托</param>
    /// <returns>应用程序构建器</returns>
    public static IApplicationBuilder UseFrameworkMiddleware(this IApplicationBuilder app, Action<IMiddlewarePipeline> configureMiddleware)
    {
        var framework = app.ApplicationServices.GetRequiredService<IApplicationFramework>();
        configureMiddleware(framework.MiddlewarePipeline);
        
        return app.UseFramework();
    }

    /// <summary>
    /// 使用框架中间件（泛型）
    /// </summary>
    /// <typeparam name="TMiddleware">中间件类型</typeparam>
    /// <param name="app">应用程序构建器</param>
    /// <returns>应用程序构建器</returns>
    public static IApplicationBuilder UseFrameworkMiddleware<TMiddleware>(this IApplicationBuilder app)
        where TMiddleware : class, IMiddleware
    {
        return app.UseFrameworkMiddleware(pipeline => pipeline.Use<TMiddleware>());
    }
}
