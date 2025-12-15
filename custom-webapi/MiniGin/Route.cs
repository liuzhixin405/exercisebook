using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniGin;

/// <summary>
/// 路由定义
/// </summary>
public sealed class Route
{
    /// <summary>
    /// 创建路由定义
    /// </summary>
    public Route(string method, string path, RoutePattern pattern, HandlerFunc[] handlers)
    {
        Method = method;
        Path = path;
        Pattern = pattern;
        Handlers = handlers;
    }

    /// <summary>HTTP 方法</summary>
    public string Method { get; }

    /// <summary>路由路径</summary>
    public string Path { get; }

    /// <summary>路由模式</summary>
    public RoutePattern Pattern { get; }

    /// <summary>处理器链</summary>
    public HandlerFunc[] Handlers { get; }

    /// <summary>OpenAPI 格式路径</summary>
    public string OpenApiPath => Path.Split('/')
        .Select(s => s.StartsWith(":") ? "{" + s[1..] + "}" : s)
        .Aggregate((a, b) => a + "/" + b);

    /// <summary>路径参数列表</summary>
    public string[] PathParameters => Path.Split('/')
        .Where(s => s.StartsWith(":"))
        .Select(s => s[1..])
        .ToArray();
}

/// <summary>
/// 路由模式解析
/// </summary>
public sealed class RoutePattern
{
    private readonly Segment[] _segments;

    private RoutePattern(Segment[] segments) => _segments = segments;

    /// <summary>
    /// 解析路由模式
    /// </summary>
    public static RoutePattern Parse(string path)
    {
        var cleaned = (path ?? "/").Trim().Trim('/');
        if (string.IsNullOrEmpty(cleaned))
            return new RoutePattern(Array.Empty<Segment>());

        var parts = cleaned.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var segments = parts.Select(ParseSegment).ToArray();
        return new RoutePattern(segments);
    }

    private static Segment ParseSegment(string part)
    {
        if (part.StartsWith(":"))
            return new Segment(true, part[1..], false);
        if (part.StartsWith("*"))
            return new Segment(true, part[1..], true);
        return new Segment(false, part, false);
    }

    /// <summary>
    /// 尝试匹配请求路径
    /// </summary>
    public bool TryMatch(string requestPath, out Dictionary<string, string> routeParams)
    {
        routeParams = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        var cleaned = (requestPath ?? "/").Trim().Trim('/');
        var parts = string.IsNullOrEmpty(cleaned)
            ? Array.Empty<string>()
            : cleaned.Split('/', StringSplitOptions.RemoveEmptyEntries);

        // 检查通配符
        var hasWildcard = _segments.Any(s => s.IsWildcard);
        if (!hasWildcard && parts.Length != _segments.Length)
            return false;

        for (var i = 0; i < _segments.Length; i++)
        {
            var segment = _segments[i];

            if (segment.IsWildcard)
            {
                // 通配符匹配剩余所有路径
                var remaining = string.Join("/", parts.Skip(i));
                routeParams[segment.Value] = Uri.UnescapeDataString(remaining);
                return true;
            }

            if (i >= parts.Length)
                return false;

            var value = parts[i];

            if (segment.IsParam)
            {
                routeParams[segment.Value] = Uri.UnescapeDataString(value);
            }
            else if (!string.Equals(segment.Value, value, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>字面量段数量（用于排序）</summary>
    public int LiteralCount => _segments.Count(s => !s.IsParam);

    private readonly record struct Segment(bool IsParam, string Value, bool IsWildcard = false);
}
