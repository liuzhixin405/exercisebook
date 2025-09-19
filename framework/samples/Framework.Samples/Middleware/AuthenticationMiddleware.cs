using Framework.Core.Abstractions.Middleware;
using Microsoft.AspNetCore.Http;
using FrameworkMiddleware = Framework.Core.Abstractions.Middleware.IMiddleware;

namespace Framework.Samples.Middleware;

/// <summary>
/// 认证中间件
/// </summary>
public class AuthenticationMiddleware : FrameworkMiddleware
{
    /// <inheritdoc />
    public string Name => "AuthenticationMiddleware";

    /// <inheritdoc />
    public int Priority => 50; // 在日志中间件之后执行

    /// <inheritdoc />
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        
        if (string.IsNullOrEmpty(authHeader))
        {
            Console.WriteLine("认证失败: 缺少Authorization头");
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized");
            return;
        }

        if (!authHeader.StartsWith("Bearer "))
        {
            Console.WriteLine("认证失败: 无效的Authorization格式");
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized");
            return;
        }

        var token = authHeader.Substring("Bearer ".Length);
        if (string.IsNullOrEmpty(token) || token != "valid-token")
        {
            Console.WriteLine("认证失败: 无效的令牌");
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized");
            return;
        }

        Console.WriteLine("认证成功");
        await next(context);
    }

    /// <inheritdoc />
    public bool ShouldExecute(HttpContext context)
    {
        // 只对需要认证的路径执行
        return !context.Request.Path.StartsWithSegments("/swagger") &&
               !context.Request.Path.StartsWithSegments("/health");
    }
}
