using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using AiDevPlatform.Core.Interfaces;

namespace AiDevPlatform.Infrastructure.Services;

public class CacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<CacheService> _logger;

    public CacheService(IMemoryCache memoryCache, ILogger<CacheService> logger)
    {
        _memoryCache = memoryCache;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            return await Task.FromResult(_memoryCache.TryGetValue(key, out T? value) ? value : default(T));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get cache value for key: {Key}", key);
            return default(T);
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        try
        {
            var options = new MemoryCacheEntryOptions
            {
                SlidingExpiration = expiration ?? TimeSpan.FromMinutes(30),
                Priority = CacheItemPriority.Normal
            };

            _memoryCache.Set(key, value, options);
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set cache value for key: {Key}", key);
        }
    }

    public async Task RemoveAsync(string key)
    {
        try
        {
            _memoryCache.Remove(key);
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove cache value for key: {Key}", key);
        }
    }

    public async Task RemoveByPatternAsync(string pattern)
    {
        try
        {
            // Note: IMemoryCache doesn't support pattern-based removal
            // This is a simplified implementation
            // In a real scenario, you might want to use a different caching solution
            _logger.LogWarning("Pattern-based cache removal is not supported by IMemoryCache");
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove cache values by pattern: {Pattern}", pattern);
        }
    }

    public async Task<bool> ExistsAsync(string key)
    {
        try
        {
            return await Task.FromResult(_memoryCache.TryGetValue(key, out _));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check cache existence for key: {Key}", key);
            return false;
        }
    }

    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
    {
        try
        {
            if (_memoryCache.TryGetValue(key, out T? cachedValue))
            {
                return cachedValue!;
            }

            var value = await factory();
            await SetAsync(key, value, expiration);
            return value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get or set cache value for key: {Key}", key);
            return await factory();
        }
    }

    public async Task<Dictionary<string, T>> GetManyAsync<T>(IEnumerable<string> keys)
    {
        try
        {
            var result = new Dictionary<string, T>();
            foreach (var key in keys)
            {
                if (_memoryCache.TryGetValue(key, out T? value))
                {
                    result[key] = value!;
                }
            }
            return await Task.FromResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get multiple cache values");
            return new Dictionary<string, T>();
        }
    }

    public async Task SetManyAsync<T>(Dictionary<string, T> values, TimeSpan? expiration = null)
    {
        try
        {
            foreach (var kvp in values)
            {
                await SetAsync(kvp.Key, kvp.Value, expiration);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set multiple cache values");
        }
    }

    public async Task RemoveManyAsync(IEnumerable<string> keys)
    {
        try
        {
            foreach (var key in keys)
            {
                await RemoveAsync(key);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove multiple cache values");
        }
    }

    public async Task ClearAsync()
    {
        try
        {
            // Note: IMemoryCache doesn't support clearing all entries
            // This is a simplified implementation
            _logger.LogWarning("Clear all cache is not supported by IMemoryCache");
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clear cache");
        }
    }

    public async Task<long> GetMemoryUsageAsync()
    {
        try
        {
            // Note: IMemoryCache doesn't provide memory usage information
            // This is a placeholder implementation
            _logger.LogWarning("Memory usage is not available for IMemoryCache");
            return await Task.FromResult(0L);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get memory usage");
            return 0;
        }
    }

    public async Task<IEnumerable<string>> GetKeysAsync(string pattern = "*")
    {
        try
        {
            // Note: IMemoryCache doesn't support key enumeration
            // This is a placeholder implementation
            _logger.LogWarning("Key enumeration is not supported by IMemoryCache");
            return await Task.FromResult(Enumerable.Empty<string>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get cache keys");
            return Enumerable.Empty<string>();
        }
    }
}
