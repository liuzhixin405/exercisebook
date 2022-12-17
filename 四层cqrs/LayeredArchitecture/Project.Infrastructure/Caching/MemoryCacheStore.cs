using Microsoft.Extensions.Caching.Memory;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.Caching
{
    public class MemoryCacheStore : ICacheStore
    {
        private readonly IMemoryCache _memoryCache;
        private readonly Dictionary<String,TimeSpan> _expirationConfiguration;
        public MemoryCacheStore(IMemoryCache memoryCache, Dictionary<String, TimeSpan> expirationConfiguration)
        {
            this._memoryCache = memoryCache;
            this._expirationConfiguration = expirationConfiguration;
        }
        public void Add<TItem>(TItem item, ICacheKey<TItem> key, TimeSpan? expirationtime = null)
        {
            var cacheObjectName = item.GetType().Name;
            TimeSpan timespan;
            if (expirationtime.HasValue)
            {
                timespan = expirationtime.Value;
            }
            else
            {
                timespan = _expirationConfiguration[cacheObjectName];
            }
            this._memoryCache.Set(key.CacheKey,item, timespan);
        }

        public void Add<TItem>(TItem item, ICacheKey<TItem> key, DateTime? absoluteExpiration = null)
        {
            DateTimeOffset offset;
            if (absoluteExpiration.HasValue)
            {
                offset = absoluteExpiration.Value;
            }
            else
            {
                offset = DateTimeOffset.MaxValue;
            }
            this._memoryCache.Set(key.CacheKey, item, offset);
        }

        public TItem Get<TItem>(ICacheKey<TItem> key) where TItem : class
        {
            if(_memoryCache.TryGetValue(key.CacheKey,out TItem value))
            {
                return value;
            }
            return null;
        }

        public void Remove<TItem>(ICacheKey<TItem> key)
        {
            _memoryCache.Remove(key.CacheKey);
        }
    }
}
