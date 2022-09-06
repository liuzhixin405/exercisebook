using AspNetCore.Extensions;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Helper
{
    public class MemoryCacheHelper : IMemoryCacheHelper
    {
        private readonly IMemoryCache memoryCache;
        public MemoryCacheHelper(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }

        private static void ValidateValueType<TResult>()
        {
            Type typeResult = typeof(TResult);
            if (typeResult.IsGenericType)
            {
                typeResult = typeResult.GetGenericTypeDefinition();
                if(typeResult==typeof(IEnumerable<TResult>)
                    || typeResult==typeof(IEnumerable)
                    ||typeResult==typeof(IAsyncEnumerable<TResult>)
                    ||typeResult==typeof(IQueryable<TResult>)
                    ||typeResult==typeof(IQueryable))
                {
                    throw new InvalidOperationException($"TResult of {typeResult} is not allowed, please use List<T> or T[] instead.");
                }
            }
        }
        private static void InitCacheEntry(ICacheEntry entry,int baseExpireSeconds)
        {
            double sec = new Random().NextDouble(baseExpireSeconds,baseExpireSeconds*2);
            TimeSpan expiration =TimeSpan.FromSeconds(sec);
            entry.AbsoluteExpirationRelativeToNow = expiration;
        }
        public TResult? GetOrCreate<TResult>(string cacheKey, Func<ICacheEntry, TResult?> valueFactory, int expireSeconds = 60)
        {
            ValidateValueType<TResult>();
            if(!memoryCache.TryGetValue(cacheKey, out TResult result))
            {
                using ICacheEntry entry = memoryCache.CreateEntry(cacheKey);
                InitCacheEntry(entry, expireSeconds);
                result= valueFactory(entry)!;
                entry.Value = result;
            }
            return result;
        }

        public async Task<TResult?> GetOrCreateAsync<TResult>(string cacheKey, Func<ICacheEntry, Task<TResult?>> valueFactory, int expireSeconds = 60)
        {

            ValidateValueType<TResult>();
            if (!memoryCache.TryGetValue(cacheKey, out TResult result))
            {
                using ICacheEntry entry = memoryCache.CreateEntry(cacheKey);
                InitCacheEntry(entry, expireSeconds);
                result =(await valueFactory(entry))!;
                entry.Value = result;
            }
            return result;
        }

        public void Remove(string cacheKey)
        {
            memoryCache.Remove(cacheKey);
        }
    }
}
