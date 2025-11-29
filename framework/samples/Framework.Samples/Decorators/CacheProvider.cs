using Framework.Infrastructure.Decorators;
using System.Collections.Concurrent;

namespace Framework.Samples.Decorators;

/// <summary>
/// 缓存提供者实现 - 装饰器模式示例
/// </summary>
public class CacheProvider : ICacheProvider
{
    private readonly ConcurrentDictionary<string, CacheEntry> _cache = new();

    public T? Get<T>(string key)
    {
        if (_cache.TryGetValue(key, out var entry))
        {
            if (entry.ExpiresAt == null || entry.ExpiresAt > DateTime.UtcNow)
            {
                Console.WriteLine($"[缓存命中] 键: {key}");
                return (T?)entry.Value;
            }
            else
            {
                _cache.TryRemove(key, out _);
                Console.WriteLine($"[缓存过期] 键: {key}");
            }
        }
        Console.WriteLine($"[缓存未命中] 键: {key}");
        return default;
    }

    public void Set<T>(string key, T value, TimeSpan? expiration = null)
    {
        var expiresAt = expiration.HasValue ? DateTime.UtcNow.Add(expiration.Value) : (DateTime?)null;
        _cache[key] = new CacheEntry(value, expiresAt);
        Console.WriteLine($"[缓存设置] 键: {key}, 过期时间: {(expiration.HasValue ? expiration.Value.ToString() : "无")}");
    }

    public void Remove(string key)
    {
        if (_cache.TryRemove(key, out _))
        {
            Console.WriteLine($"[缓存移除] 键: {key}");
        }
    }

    private class CacheEntry
    {
        public object? Value { get; }
        public DateTime? ExpiresAt { get; }

        public CacheEntry(object? value, DateTime? expiresAt)
        {
            Value = value;
            ExpiresAt = expiresAt;
        }
    }
}
