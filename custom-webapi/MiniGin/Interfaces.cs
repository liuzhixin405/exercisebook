using System.Threading.Tasks;

namespace MiniGin;

/// <summary>
/// 请求处理函数委托
/// </summary>
/// <param name="ctx">请求上下文</param>
public delegate Task HandlerFunc(Context ctx);

/// <summary>
/// 中间件接口
/// </summary>
public interface IMiddleware
{
    /// <summary>
    /// 执行中间件逻辑
    /// </summary>
    /// <param name="ctx">请求上下文</param>
    /// <param name="next">下一个处理器</param>
    Task InvokeAsync(Context ctx, HandlerFunc next);
}

/// <summary>
/// 路由器接口 - 定义路由注册能力
/// </summary>
public interface IRouter
{
    /// <summary>
    /// 注册中间件
    /// </summary>
    IRouter Use(params IMiddleware[] middleware);

    /// <summary>
    /// 注册处理函数作为中间件
    /// </summary>
    IRouter Use(params HandlerFunc[] handlers);

    /// <summary>
    /// 注册 GET 路由
    /// </summary>
    IRouter GET(string path, params HandlerFunc[] handlers);

    /// <summary>
    /// 注册 POST 路由
    /// </summary>
    IRouter POST(string path, params HandlerFunc[] handlers);

    /// <summary>
    /// 注册 PUT 路由
    /// </summary>
    IRouter PUT(string path, params HandlerFunc[] handlers);

    /// <summary>
    /// 注册 DELETE 路由
    /// </summary>
    IRouter DELETE(string path, params HandlerFunc[] handlers);

    /// <summary>
    /// 注册 PATCH 路由
    /// </summary>
    IRouter PATCH(string path, params HandlerFunc[] handlers);

    /// <summary>
    /// 注册 HEAD 路由
    /// </summary>
    IRouter HEAD(string path, params HandlerFunc[] handlers);

    /// <summary>
    /// 注册 OPTIONS 路由
    /// </summary>
    IRouter OPTIONS(string path, params HandlerFunc[] handlers);

    /// <summary>
    /// 注册指定 HTTP 方法的路由
    /// </summary>
    IRouter Handle(string method, string path, params HandlerFunc[] handlers);

    /// <summary>
    /// 创建路由分组
    /// </summary>
    RouterGroup Group(string relativePath);
}

/// <summary>
/// 中间件基类
/// </summary>
public abstract class MiddlewareBase : IMiddleware
{
    /// <inheritdoc />
    public abstract Task InvokeAsync(Context ctx, HandlerFunc next);
}
