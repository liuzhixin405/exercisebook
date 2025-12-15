using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MiniGin;

/// <summary>
/// 请求上下文 - 封装 HTTP 请求/响应的所有操作
/// </summary>
public sealed class Context
{
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly Dictionary<string, string> _params;
    private readonly Dictionary<string, object> _items = new();
    private bool _responseSent;
    private string? _cachedBody;

    internal Context(HttpListenerContext httpContext, Dictionary<string, string> routeParams, JsonSerializerOptions jsonOptions)
    {
        HttpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
        _params = routeParams ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        _jsonOptions = jsonOptions;
    }

    #region 基础属性

    /// <summary>原始 HttpListenerContext</summary>
    public HttpListenerContext HttpContext { get; }

    /// <summary>HTTP 请求对象</summary>
    public HttpListenerRequest Request => HttpContext.Request;

    /// <summary>HTTP 响应对象</summary>
    public HttpListenerResponse Response => HttpContext.Response;

    /// <summary>请求路径</summary>
    public string Path => Request.Url?.AbsolutePath ?? "/";

    /// <summary>请求方法</summary>
    public string Method => Request.HttpMethod ?? "GET";

    /// <summary>完整 URL</summary>
    public string FullUrl => Request.Url?.ToString() ?? "";

    /// <summary>客户端 IP</summary>
    public string ClientIP => Request.RemoteEndPoint?.Address?.ToString() ?? "";

    /// <summary>Content-Type</summary>
    public string? ContentType => Request.ContentType;

    /// <summary>是否已中止</summary>
    public bool IsAborted { get; private set; }

    #endregion

    #region 路由参数

    /// <summary>获取路由参数</summary>
    public string? Param(string key)
        => _params.TryGetValue(key, out var value) ? value : null;

    /// <summary>获取路由参数（带默认值）</summary>
    public string Param(string key, string defaultValue)
        => _params.TryGetValue(key, out var value) ? value : defaultValue;

    /// <summary>获取所有路由参数</summary>
    public IReadOnlyDictionary<string, string> Params => _params;

    #endregion

    #region 查询参数

    /// <summary>获取查询参数</summary>
    public string? Query(string key)
        => Request.QueryString[key];

    /// <summary>获取查询参数（带默认值）</summary>
    public string Query(string key, string defaultValue)
        => Request.QueryString[key] ?? defaultValue;

    /// <summary>获取查询参数并转换类型</summary>
    public T? Query<T>(string key) where T : struct
    {
        var value = Request.QueryString[key];
        if (string.IsNullOrEmpty(value)) return null;

        try
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
        catch
        {
            return null;
        }
    }

    /// <summary>获取所有查询参数的 key</summary>
    public string[] QueryKeys => Request.QueryString.AllKeys!;

    #endregion

    #region 请求头

    /// <summary>获取请求头</summary>
    public string? GetHeader(string key)
        => Request.Headers[key];

    /// <summary>获取请求头（带默认值）</summary>
    public string GetHeader(string key, string defaultValue)
        => Request.Headers[key] ?? defaultValue;

    #endregion

    #region 请求体

    /// <summary>读取原始请求体</summary>
    public async Task<string> GetRawBodyAsync()
    {
        if (_cachedBody != null)
            return _cachedBody;

        if (!Request.HasEntityBody)
            return _cachedBody = string.Empty;

        using var reader = new StreamReader(Request.InputStream, Request.ContentEncoding ?? Encoding.UTF8);
        return _cachedBody = await reader.ReadToEndAsync();
    }

    /// <summary>绑定 JSON 请求体到对象</summary>
    public async Task<T?> BindAsync<T>() where T : class
    {
        var body = await GetRawBodyAsync();
        if (string.IsNullOrWhiteSpace(body))
            return null;

        return JsonSerializer.Deserialize<T>(body, _jsonOptions);
    }

    /// <summary>绑定 JSON 请求体到对象（带默认值）</summary>
    public async Task<T> BindAsync<T>(T defaultValue) where T : class
    {
        var result = await BindAsync<T>();
        return result ?? defaultValue;
    }

    /// <summary>必须绑定成功，否则抛异常</summary>
    public async Task<T> MustBindAsync<T>() where T : class
    {
        var result = await BindAsync<T>();
        return result ?? throw new InvalidOperationException($"Failed to bind request body to {typeof(T).Name}");
    }

    #endregion

    #region 上下文数据

    /// <summary>设置上下文数据</summary>
    public void Set(string key, object value) => _items[key] = value;

    /// <summary>获取上下文数据</summary>
    public T? Get<T>(string key) where T : class
        => _items.TryGetValue(key, out var value) ? value as T : null;

    /// <summary>获取上下文数据（带默认值）</summary>
    public T Get<T>(string key, T defaultValue) where T : class
        => _items.TryGetValue(key, out var value) && value is T typed ? typed : defaultValue;

    /// <summary>是否存在上下文数据</summary>
    public bool Has(string key) => _items.ContainsKey(key);

    #endregion

    #region 响应方法

    /// <summary>中止请求处理</summary>
    public void Abort() => IsAborted = true;

    /// <summary>设置响应头</summary>
    public Context Header(string key, string value)
    {
        Response.Headers[key] = value;
        return this;
    }

    /// <summary>设置状态码并结束响应</summary>
    public Task Status(int statusCode)
    {
        if (!TryStartResponse()) return Task.CompletedTask;

        Response.StatusCode = statusCode;
        Response.ContentLength64 = 0;
        Response.OutputStream.Close();
        return Task.CompletedTask;
    }

    /// <summary>返回纯文本</summary>
    public Task String(int statusCode, string content)
    {
        if (!TryStartResponse()) return Task.CompletedTask;

        var bytes = Encoding.UTF8.GetBytes(content);
        Response.StatusCode = statusCode;
        Response.ContentType = "text/plain; charset=utf-8";
        Response.ContentLength64 = bytes.Length;
        return WriteAndCloseAsync(bytes);
    }

    /// <summary>返回 HTML</summary>
    public Task HTML(int statusCode, string html)
    {
        if (!TryStartResponse()) return Task.CompletedTask;

        var bytes = Encoding.UTF8.GetBytes(html);
        Response.StatusCode = statusCode;
        Response.ContentType = "text/html; charset=utf-8";
        Response.ContentLength64 = bytes.Length;
        return WriteAndCloseAsync(bytes);
    }

    /// <summary>返回 JSON</summary>
    public Task JSON(int statusCode, object? data)
    {
        if (!TryStartResponse()) return Task.CompletedTask;

        var bytes = JsonSerializer.SerializeToUtf8Bytes(data, _jsonOptions);
        Response.StatusCode = statusCode;
        Response.ContentType = "application/json; charset=utf-8";
        Response.ContentLength64 = bytes.Length;
        return WriteAndCloseAsync(bytes);
    }

    /// <summary>返回 JSON（200 状态码）</summary>
    public Task JSON(object? data) => JSON(200, data);

    /// <summary>返回原始字节</summary>
    public Task Data(int statusCode, string contentType, byte[] data)
    {
        if (!TryStartResponse()) return Task.CompletedTask;

        Response.StatusCode = statusCode;
        Response.ContentType = contentType;
        Response.ContentLength64 = data.Length;
        return WriteAndCloseAsync(data);
    }

    /// <summary>重定向</summary>
    public Task Redirect(int statusCode, string location)
    {
        if (!TryStartResponse()) return Task.CompletedTask;

        Response.StatusCode = statusCode;
        Response.RedirectLocation = location;
        Response.ContentLength64 = 0;
        Response.OutputStream.Close();
        return Task.CompletedTask;
    }

    /// <summary>重定向（302）</summary>
    public Task Redirect(string location) => Redirect(302, location);

    #endregion

    #region 快捷响应方法

    /// <summary>200 OK</summary>
    public Task OK(object? data = null) => data == null ? Status(200) : JSON(200, data);

    /// <summary>201 Created</summary>
    public Task Created(object? data = null) => data == null ? Status(201) : JSON(201, data);

    /// <summary>204 No Content</summary>
    public Task NoContent() => Status(204);

    /// <summary>400 Bad Request</summary>
    public Task BadRequest(object? error = null)
        => JSON(400, error ?? new { error = "Bad Request" });

    /// <summary>401 Unauthorized</summary>
    public Task Unauthorized(object? error = null)
        => JSON(401, error ?? new { error = "Unauthorized" });

    /// <summary>403 Forbidden</summary>
    public Task Forbidden(object? error = null)
        => JSON(403, error ?? new { error = "Forbidden" });

    /// <summary>404 Not Found</summary>
    public Task NotFound(object? error = null)
        => JSON(404, error ?? new { error = "Not Found" });

    /// <summary>500 Internal Server Error</summary>
    public Task InternalServerError(object? error = null)
        => JSON(500, error ?? new { error = "Internal Server Error" });

    /// <summary>中止并返回状态码</summary>
    public Task AbortWithStatus(int statusCode)
    {
        Abort();
        return Status(statusCode);
    }

    /// <summary>中止并返回 JSON 错误</summary>
    public Task AbortWithJSON(int statusCode, object error)
    {
        Abort();
        return JSON(statusCode, error);
    }

    #endregion

    #region 私有方法

    private bool TryStartResponse()
    {
        if (_responseSent) return false;
        _responseSent = true;
        return true;
    }

    private async Task WriteAndCloseAsync(byte[] bytes)
    {
        await Response.OutputStream.WriteAsync(bytes, 0, bytes.Length);
        Response.OutputStream.Close();
    }

    #endregion
}
