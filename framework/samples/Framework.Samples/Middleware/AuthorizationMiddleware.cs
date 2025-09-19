using Framework.Core.Abstractions.Middleware;
using Microsoft.AspNetCore.Http;
using FrameworkMiddleware = Framework.Core.Abstractions.Middleware.IMiddleware;

namespace Framework.Samples.Middleware;

/// <summary>
/// 授权中间件
/// </summary>
public class AuthorizationMiddleware : FrameworkMiddleware
{
    /// <inheritdoc />
    public string Name => "AuthorizationMiddleware";

    /// <inheritdoc />
    public int Priority => 25; // 在认证中间件之后执行

    /// <inheritdoc />
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var userRole = context.Request.Headers["X-User-Role"].FirstOrDefault();
        
        if (string.IsNullOrEmpty(userRole))
        {
            Console.WriteLine("授权失败: 缺少用户角色");
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("Forbidden");
            return;
        }

        // 检查用户是否有权限访问当前路径
        if (context.Request.Path.StartsWithSegments("/admin") && userRole != "admin")
        {
            Console.WriteLine($"授权失败: 用户角色 {userRole} 无权访问管理员路径");
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("Forbidden");
            return;
        }

        Console.WriteLine($"授权成功: 用户角色 {userRole}");
        await next(context);
    }

    /// <inheritdoc />
    public bool ShouldExecute(HttpContext context)
    {
        // 只对需要授权的路径执行
        return !context.Request.Path.StartsWithSegments("/swagger") &&
               !context.Request.Path.StartsWithSegments("/health") &&
               !context.Request.Path.StartsWithSegments("/public");
    }
}
