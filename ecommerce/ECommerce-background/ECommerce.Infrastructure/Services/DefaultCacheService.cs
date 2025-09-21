using ECommerce.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace ECommerce.Infrastructure.Services
{
    public class DefaultCacheService : ICacheService
    {
        private readonly ILogger<DefaultCacheService> _logger;
        private readonly ConcurrentDictionary<string, (object Value, DateTime Expiration)> _cache;

        public DefaultCacheService(ILogger<DefaultCacheService> logger)
        {
            _logger = logger;
            _cache = new ConcurrentDictionary<string, (object, DateTime)>();
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            try
            {
                if (_cache.TryGetValue(key, out var cacheItem))
                {
                    if (cacheItem.Expiration > DateTime.UtcNow)
                    {
                        _logger.LogDebug("Cache hit for key: {Key}", key);
                        return (T)cacheItem.Value;
                    }
                    else
                    {
                        // 缓存已过期，移除
                        _cache.TryRemove(key, out _);
                        _logger.LogDebug("Cache expired for key: {Key}", key);
                    }
                }

                _logger.LogDebug("Cache miss for key: {Key}", key);
                return default(T);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cache for key: {Key}", key);
                return default(T);
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            try
            {
                var expirationTime = DateTime.UtcNow.Add(expiration ?? TimeSpan.FromMinutes(30));
                _cache.AddOrUpdate(key, (value, expirationTime), (k, v) => (value, expirationTime));
                
                _logger.LogDebug("Cache set for key: {Key} with expiration: {Expiration}", key, expirationTime);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting cache for key: {Key}", key);
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                if (_cache.TryRemove(key, out _))
                {
                    _logger.LogDebug("Cache removed for key: {Key}", key);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache for key: {Key}", key);
            }
        }

        public async Task RemoveByPatternAsync(string pattern)
        {
            try
            {
                var keysToRemove = _cache.Keys.Where(k => k.Contains(pattern)).ToList();
                foreach (var key in keysToRemove)
                {
                    _cache.TryRemove(key, out _);
                }
                
                _logger.LogDebug("Cache removed {Count} keys matching pattern: {Pattern}", keysToRemove.Count, pattern);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache by pattern: {Pattern}", pattern);
            }
        }

        public async Task<bool> ExistsAsync(string key)
        {
            try
            {
                if (_cache.TryGetValue(key, out var cacheItem))
                {
                    return cacheItem.Expiration > DateTime.UtcNow;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking cache existence for key: {Key}", key);
                return false;
            }
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
        {
            try
            {
                var cachedValue = await GetAsync<T>(key);
                if (cachedValue != null)
                {
                    return cachedValue;
                }

                var newValue = await factory();
                await SetAsync(key, newValue, expiration);
                return newValue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetOrSet for key: {Key}", key);
                throw;
            }
        }
    }
}
