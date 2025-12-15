using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MiniGin;

/// <summary>
/// Gin 风格的 HTTP 引擎 - 核心入口
/// </summary>
public class Engine : RouterGroup
{
    private readonly List<Route> _routes = new();
    private readonly JsonSerializerOptions _jsonOptions;
    private HttpListener? _listener;

    private bool _swaggerEnabled;
    private string _swaggerTitle = "MiniGin API";
    private string _swaggerVersion = "v1";

    /// <summary>
    /// 创建新的引擎实例
    /// </summary>
    public Engine() : this(new JsonSerializerOptions(JsonSerializerDefaults.Web))
    {
    }

    /// <summary>
    /// 创建新的引擎实例（自定义 JSON 选项）
    /// </summary>
    public Engine(JsonSerializerOptions jsonOptions) : base(null!, "")
    {
        _jsonOptions = jsonOptions;
        SetEngine(this);
    }

    private void SetEngine(Engine engine)
    {
        var field = typeof(RouterGroup).GetField("_engine", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(this, engine);
    }

    #region 配置

    /// <summary>
    /// 启用 Swagger UI 和 OpenAPI 文档
    /// </summary>
    /// <param name="title">API 标题</param>
    /// <param name="version">API 版本</param>
    public Engine UseSwagger(string title = "MiniGin API", string version = "v1")
    {
        _swaggerEnabled = true;
        _swaggerTitle = title;
        _swaggerVersion = version;
        return this;
    }

    /// <summary>
    /// 获取所有已注册的路由
    /// </summary>
    public IReadOnlyList<Route> Routes => _routes;

    /// <summary>
    /// JSON 序列化选项
    /// </summary>
    public JsonSerializerOptions JsonOptions => _jsonOptions;

    #endregion

    #region 路由注册（内部）

    internal void AddRoute(string method, string path, HandlerFunc[] handlers)
    {
        if (string.IsNullOrWhiteSpace(method))
            throw new ArgumentException("HTTP method is required.", nameof(method));

        if (handlers == null || handlers.Length == 0)
            throw new ArgumentException("At least one handler is required.", nameof(handlers));

        var pattern = RoutePattern.Parse(path);
        var route = new Route(method.ToUpperInvariant(), path, pattern, handlers);

        _routes.Add(route);
        _routes.Sort((a, b) => b.Pattern.LiteralCount.CompareTo(a.Pattern.LiteralCount));
    }

    #endregion

    #region 运行

    /// <summary>
    /// 启动 HTTP 服务器
    /// </summary>
    /// <param name="address">监听地址，如 http://localhost:5000/</param>
    public Task Run(string address = "http://localhost:5000/")
        => Run(address, CancellationToken.None);

    /// <summary>
    /// 启动 HTTP 服务器（支持取消）
    /// </summary>
    /// <param name="address">监听地址</param>
    /// <param name="cancellationToken">取消令牌</param>
    public async Task Run(string address, CancellationToken cancellationToken)
    {
        if (!address.EndsWith("/"))
            address += "/";

        _listener = new HttpListener();
        _listener.Prefixes.Add(address);
        _listener.Start();

        Console.WriteLine($"[MiniGin] Listening on {address}");
        if (_swaggerEnabled)
            Console.WriteLine($"[MiniGin] Swagger UI: {address}swagger");

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var httpContext = await _listener.GetContextAsync();
                    _ = Task.Run(() => HandleRequestAsync(httpContext), cancellationToken);
                }
                catch (Exception ex) when (!(ex is HttpListenerException))
                {
                    Console.WriteLine($"[MiniGin] Error accepting connection: {ex.Message}");
                }
            }
        }
        catch (HttpListenerException) when (cancellationToken.IsCancellationRequested)
        {
            // 正常关闭
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[MiniGin] Fatal error: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            throw;
        }
        finally
        {
            _listener.Stop();
            _listener.Close();
        }
    }

    /// <summary>
    /// 停止服务器
    /// </summary>
    public void Stop()
    {
        _listener?.Stop();
    }

    private async Task HandleRequestAsync(HttpListenerContext httpContext)
    {
        try
        {
            var path = httpContext.Request.Url?.AbsolutePath ?? "/";
            var method = httpContext.Request.HttpMethod ?? "GET";

            // 处理 Swagger
            if (_swaggerEnabled && await TryHandleSwaggerAsync(httpContext, path))
                return;

            // 查找路由
            var (route, routeParams) = FindRoute(method, path);
            if (route == null)
            {
                await WriteNotFound(httpContext.Response);
                return;
            }

            // 创建上下文
            var ctx = new Context(httpContext, routeParams, _jsonOptions);

            // 执行处理器链
            await ExecuteHandlers(ctx, route.Handlers);
        }
        catch (Exception ex)
        {
            await WriteError(httpContext.Response, ex);
        }
    }

    private async Task ExecuteHandlers(Context ctx, HandlerFunc[] handlers)
    {
        foreach (var handler in handlers)
        {
            if (ctx.IsAborted)
                break;

            await handler(ctx);
        }
    }

    private (Route? route, Dictionary<string, string> routeParams) FindRoute(string method, string path)
    {
        foreach (var route in _routes)
        {
            if (!string.Equals(route.Method, method, StringComparison.OrdinalIgnoreCase))
                continue;

            if (route.Pattern.TryMatch(path, out var routeParams))
                return (route, routeParams);
        }

        return (null, new Dictionary<string, string>());
    }

    #endregion

    #region Swagger

    private async Task<bool> TryHandleSwaggerAsync(HttpListenerContext context, string path)
    {
        if (path.Equals("/swagger", StringComparison.OrdinalIgnoreCase) ||
            path.Equals("/swagger/", StringComparison.OrdinalIgnoreCase))
        {
            var html = GenerateSwaggerHtml();
            await WriteResponse(context.Response, 200, "text/html; charset=utf-8", html);
            return true;
        }

        if (path.Equals("/swagger/v1/swagger.json", StringComparison.OrdinalIgnoreCase))
        {
            var doc = GenerateOpenApiDoc();
            var json = JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true });
            await WriteResponse(context.Response, 200, "application/json; charset=utf-8", json);
            return true;
        }

        return false;
    }

    private object GenerateOpenApiDoc()
    {
        var paths = new Dictionary<string, object>();

        foreach (var routeGroup in _routes.GroupBy(r => r.OpenApiPath))
        {
            var operations = new Dictionary<string, object>();
            foreach (var route in routeGroup)
            {
                operations[route.Method.ToLowerInvariant()] = new
                {
                    operationId = $"{route.Method}_{route.Path.Replace("/", "_").Replace(":", "")}",
                    parameters = route.PathParameters.Select(p => new
                    {
                        name = p,
                        @in = "path",
                        required = true,
                        schema = new { type = "string" }
                    }).ToArray(),
                    responses = new Dictionary<string, object>
                    {
                        ["200"] = new { description = "OK" }
                    }
                };
            }
            paths[routeGroup.Key] = operations;
        }

        return new
        {
            openapi = "3.0.1",
            info = new { title = _swaggerTitle, version = _swaggerVersion },
            paths
        };
    }

    private static string GenerateSwaggerHtml() => @"<!doctype html>
<html>
<head>
    <meta charset=""utf-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1"" />
    <title>Swagger UI</title>
    <link rel=""stylesheet"" href=""https://unpkg.com/swagger-ui-dist@5/swagger-ui.css"" />
</head>
<body>
    <div id=""swagger-ui""></div>
    <script src=""https://unpkg.com/swagger-ui-dist@5/swagger-ui-bundle.js""></script>
    <script>
        window.onload = () => {
            SwaggerUIBundle({
                url: '/swagger/v1/swagger.json',
                dom_id: '#swagger-ui',
            });
        };
    </script>
</body>
</html>";

    #endregion

    #region 响应辅助

    private static async Task WriteResponse(HttpListenerResponse response, int statusCode, string contentType, string body)
    {
        var bytes = Encoding.UTF8.GetBytes(body);
        response.StatusCode = statusCode;
        response.ContentType = contentType;
        response.ContentLength64 = bytes.Length;
        await response.OutputStream.WriteAsync(bytes, 0, bytes.Length);
        response.OutputStream.Close();
    }

    private static Task WriteNotFound(HttpListenerResponse response)
        => WriteResponse(response, 404, "application/json", "{\"error\":\"Not Found\"}");

    private static Task WriteError(HttpListenerResponse response, Exception ex)
        => WriteResponse(response, 500, "application/json", $"{{\"error\":\"{ex.Message.Replace("\"", "\\\"")}\"}}");

    #endregion
}
