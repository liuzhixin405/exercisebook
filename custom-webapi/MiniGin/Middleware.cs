using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MiniGin;

/// <summary>
/// 内置中间件集合
/// </summary>
public static class Middleware
{
    /// <summary>
    /// 请求日志中间件
    /// </summary>
    /// <param name="logger">自定义日志输出（默认 Console.WriteLine）</param>
    public static HandlerFunc Logger(Action<string>? logger = null)
    {
        logger ??= Console.WriteLine;

        return ctx =>
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            logger($"[{timestamp}] {ctx.Method} {ctx.Path} from {ctx.ClientIP}");
            return Task.CompletedTask;
        };
    }

    /// <summary>
    /// 请求计时中间件
    /// </summary>
    /// <param name="callback">计时回调</param>
    public static HandlerFunc Timer(Action<Context, long>? callback = null)
    {
        callback ??= (ctx, ms) => Console.WriteLine($"[Timer] {ctx.Method} {ctx.Path} - {ms}ms");

        return ctx =>
        {
            var sw = Stopwatch.StartNew();
            ctx.Set("__timer_start", sw);
            ctx.Set("__timer_callback", (Action)(() =>
            {
                sw.Stop();
                callback(ctx, sw.ElapsedMilliseconds);
            }));
            return Task.CompletedTask;
        };
    }

    /// <summary>
    /// 错误恢复中间件
    /// </summary>
    /// <param name="showStackTrace">是否显示堆栈跟踪</param>
    public static HandlerFunc Recovery(bool showStackTrace = false)
    {
        return async ctx =>
        {
            try
            {
                // 预留用于自定义错误处理
            }
            catch (Exception ex)
            {
                var message = showStackTrace ? ex.ToString() : ex.Message;
                await ctx.JSON(500, new
                {
                    error = true,
                    message,
                    timestamp = DateTime.UtcNow
                });
                ctx.Abort();
            }
        };
    }

    /// <summary>
    /// CORS 中间件
    /// </summary>
    /// <param name="config">CORS 配置</param>
    public static HandlerFunc CORS(CorsConfig? config = null)
    {
        config ??= new CorsConfig();

        return async ctx =>
        {
            ctx.Header("Access-Control-Allow-Origin", config.AllowOrigins)
               .Header("Access-Control-Allow-Methods", config.AllowMethods)
               .Header("Access-Control-Allow-Headers", config.AllowHeaders);

            if (config.AllowCredentials)
                ctx.Header("Access-Control-Allow-Credentials", "true");

            if (config.MaxAge > 0)
                ctx.Header("Access-Control-Max-Age", config.MaxAge.ToString());

            // 预检请求直接返回
            if (ctx.Method == "OPTIONS")
            {
                await ctx.Status(204);
                ctx.Abort();
            }
        };
    }

    /// <summary>
    /// HTTP Basic 认证中间件
    /// </summary>
    /// <param name="validator">用户名密码验证器</param>
    /// <param name="realm">认证域</param>
    public static HandlerFunc BasicAuth(Func<string, string, bool> validator, string realm = "Authorization Required")
    {
        return async ctx =>
        {
            var authHeader = ctx.GetHeader("Authorization");
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Basic "))
            {
                ctx.Header("WWW-Authenticate", $"Basic realm=\"{realm}\"");
                await ctx.Unauthorized(new { error = "Unauthorized" });
                ctx.Abort();
                return;
            }

            try
            {
                var encoded = authHeader["Basic ".Length..];
                var decoded = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
                var parts = decoded.Split(':', 2);

                if (parts.Length != 2 || !validator(parts[0], parts[1]))
                {
                    await ctx.Unauthorized(new { error = "Invalid credentials" });
                    ctx.Abort();
                }
                else
                {
                    ctx.Set("user", parts[0]);
                }
            }
            catch
            {
                await ctx.Unauthorized(new { error = "Invalid authorization header" });
                ctx.Abort();
            }
        };
    }

    /// <summary>
    /// API Key 认证中间件
    /// </summary>
    /// <param name="headerName">请求头名称</param>
    /// <param name="validator">API Key 验证器</param>
    public static HandlerFunc ApiKey(string headerName, Func<string?, bool> validator)
    {
        return async ctx =>
        {
            var apiKey = ctx.GetHeader(headerName);
            if (!validator(apiKey))
            {
                await ctx.Unauthorized(new { error = "Invalid API Key" });
                ctx.Abort();
            }
        };
    }

    /// <summary>
    /// 请求 ID 中间件
    /// </summary>
    /// <param name="headerName">请求头名称</param>
    public static HandlerFunc RequestId(string headerName = "X-Request-ID")
    {
        return ctx =>
        {
            var requestId = ctx.GetHeader(headerName);
            if (string.IsNullOrEmpty(requestId))
                requestId = Guid.NewGuid().ToString("N");

            ctx.Set("RequestId", requestId);
            ctx.Header(headerName, requestId);
            return Task.CompletedTask;
        };
    }

    /// <summary>
    /// 自定义响应头中间件
    /// </summary>
    /// <param name="headers">响应头键值对</param>
    public static HandlerFunc Headers(params (string key, string value)[] headers)
    {
        return ctx =>
        {
            foreach (var (key, value) in headers)
                ctx.Header(key, value);
            return Task.CompletedTask;
        };
    }

    /// <summary>
    /// 静态文件中间件（简单实现）
    /// </summary>
    /// <param name="urlPrefix">URL 前缀</param>
    /// <param name="rootPath">文件系统根路径</param>
    public static HandlerFunc Static(string urlPrefix, string rootPath)
    {
        return async ctx =>
        {
            if (!ctx.Path.StartsWith(urlPrefix, StringComparison.OrdinalIgnoreCase))
                return;

            var relativePath = ctx.Path[urlPrefix.Length..].TrimStart('/');
            var filePath = System.IO.Path.Combine(rootPath, relativePath);

            if (!System.IO.File.Exists(filePath))
            {
                await ctx.NotFound();
                ctx.Abort();
                return;
            }

            var contentType = GetContentType(filePath);
            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
            await ctx.Data(200, contentType, bytes);
            ctx.Abort();
        };
    }

    private static string GetContentType(string filePath)
    {
        var ext = System.IO.Path.GetExtension(filePath).ToLowerInvariant();
        return ext switch
        {
            ".html" or ".htm" => "text/html; charset=utf-8",
            ".css" => "text/css; charset=utf-8",
            ".js" => "application/javascript; charset=utf-8",
            ".json" => "application/json; charset=utf-8",
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".svg" => "image/svg+xml",
            ".ico" => "image/x-icon",
            ".woff" => "font/woff",
            ".woff2" => "font/woff2",
            ".ttf" => "font/ttf",
            ".pdf" => "application/pdf",
            ".xml" => "application/xml",
            _ => "application/octet-stream"
        };
    }
}

/// <summary>
/// CORS 配置
/// </summary>
public class CorsConfig
{
    /// <summary>允许的源</summary>
    public string AllowOrigins { get; set; } = "*";

    /// <summary>允许的方法</summary>
    public string AllowMethods { get; set; } = "GET, POST, PUT, DELETE, PATCH, OPTIONS";

    /// <summary>允许的请求头</summary>
    public string AllowHeaders { get; set; } = "Content-Type, Authorization, X-Requested-With";

    /// <summary>是否允许携带凭据</summary>
    public bool AllowCredentials { get; set; } = false;

    /// <summary>预检请求缓存时间（秒）</summary>
    public int MaxAge { get; set; } = 86400;
}
