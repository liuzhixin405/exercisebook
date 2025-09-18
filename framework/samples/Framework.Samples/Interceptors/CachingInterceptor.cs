using Framework.Core.Abstractions.Proxies;

namespace Framework.Samples.Interceptors;

/// <summary>
/// 缓存拦截器
/// </summary>
public class CachingInterceptor : IInterceptor
{
    private readonly Dictionary<string, object> _cache = new();

    /// <inheritdoc />
    public string Name => "CachingInterceptor";

    /// <inheritdoc />
    public int Priority => 50; // 在日志拦截器之前执行

    /// <inheritdoc />
    public async Task InterceptAsync(IInvocation invocation)
    {
        var methodName = $"{invocation.Target.GetType().Name}.{invocation.Method.Name}";
        var cacheKey = GenerateCacheKey(methodName, invocation.Arguments);

        // 检查缓存
        if (_cache.TryGetValue(cacheKey, out var cachedResult))
        {
            Console.WriteLine($"缓存命中: {methodName}");
            invocation.ReturnValue = cachedResult;
            invocation.IsHandled = true;
            return;
        }

        // 执行方法
        await invocation.ProceedAsync();

        // 缓存结果（只缓存非空结果）
        if (invocation.ReturnValue != null)
        {
            _cache[cacheKey] = invocation.ReturnValue;
            Console.WriteLine($"结果已缓存: {methodName}");
        }
    }

    /// <inheritdoc />
    public bool ShouldIntercept(IInvocation invocation)
    {
        // 只拦截读取方法（以Get开头的方法）
        return invocation.Method.Name.StartsWith("Get");
    }

    /// <summary>
    /// 生成缓存键
    /// </summary>
    /// <param name="methodName">方法名</param>
    /// <param name="arguments">参数</param>
    /// <returns>缓存键</returns>
    private static string GenerateCacheKey(string methodName, object[] arguments)
    {
        var argsString = string.Join("|", arguments.Select(arg => arg?.ToString() ?? "null"));
        return $"{methodName}:{argsString}";
    }
}
