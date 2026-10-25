namespace AiDevPlatform.Core.Interfaces;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
    Task RemoveAsync(string key);
    Task RemoveByPatternAsync(string pattern);
    Task<bool> ExistsAsync(string key);
    Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null);
    Task<Dictionary<string, T>> GetManyAsync<T>(IEnumerable<string> keys);
    Task SetManyAsync<T>(Dictionary<string, T> values, TimeSpan? expiration = null);
    Task RemoveManyAsync(IEnumerable<string> keys);
    Task ClearAsync();
    Task<long> GetMemoryUsageAsync();
    Task<IEnumerable<string>> GetKeysAsync(string pattern = "*");
}
