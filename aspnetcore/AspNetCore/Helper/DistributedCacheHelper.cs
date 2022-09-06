using AspNetCore.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace AspNetCore.Helper
{
    public class DistributedCacheHelper : IDistributedCacheHelper
    {
        private readonly IDistributedCache distCache;
        public DistributedCacheHelper(IDistributedCache distCache)
        {
            this.distCache = distCache;
        }
        private static DistributedCacheEntryOptions Createoptions(int baseExpiredSeconds)
        {
            double sec = new Random().NextDouble(baseExpiredSeconds, baseExpiredSeconds * 2);
            TimeSpan expiration = TimeSpan.FromSeconds(sec);
            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions();
            options.AbsoluteExpirationRelativeToNow = expiration;
            return options;
        }
        public TResult? GetOrCreate<TResult>(string cacheKey, Func<DistributedCacheEntryOptions, TResult?> valueFactory, int expireSeconds = 60)
        {
            string josnStr = distCache.GetString(cacheKey);
            if (string.IsNullOrEmpty(josnStr))
            {
                var options = Createoptions(expireSeconds);
                TResult? result = valueFactory(options);
                string josnResult = JsonSerializer.Serialize(result);
                distCache.SetString(cacheKey, josnStr, options);
                return result;
            }
            else
            {
                distCache.Refresh(cacheKey);
                return JsonSerializer.Deserialize<TResult>(josnStr);
            }
        }

        public async Task<TResult?> GetOrCreateAsync<TResult>(string cacheKey, Func<DistributedCacheEntryOptions, Task<TResult?>> valueFactory, int expireSeconds = 60)
        {
            string josnStr = await distCache.GetStringAsync(cacheKey);
            if (string.IsNullOrEmpty(josnStr))
            {
                var options = Createoptions(expireSeconds);
                TResult? result = await valueFactory(options);
                string josnResult = JsonSerializer.Serialize(result);
                distCache.SetString(cacheKey, josnStr, options);
                return result;
            }
            else
            {
                await distCache.RefreshAsync(cacheKey);
                return JsonSerializer.Deserialize<TResult>(josnStr);
            }
        }

        public void Remove(string cacheKey)
        {
            distCache.Remove(cacheKey);
        }

        public Task RemoveAsync(string cacheKey)
        {
            return distCache.RemoveAsync(cacheKey);
        }
    }
}
