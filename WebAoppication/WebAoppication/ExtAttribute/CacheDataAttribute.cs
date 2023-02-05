using CacheManager.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAoppication
{
    public class CacheDataAttribute:BaseAOPAttribute
    {
        private const string PREFIX = "_busAOPCacheKey_";
        private ICacheManager<object> _cacheManager = null;
        private string _cacheKey = string.Empty;
        public bool IsRedisNeverExpires { set; get; } = false;

        public CacheDataAttribute(bool isRedisNeverExpires = false)
        {
            IsRedisNeverExpires = isRedisNeverExpires;
        }

        public async Task<T> OnExecuting<T>(IAOPContext context)
        {
            if (!(context.Arguments[0] is ICacheKey))
                throw new NotSupportedException("该方法不支持缓存.");
            var cacheManagerConfigurations = context.ServiceProvider.GetServices<ICacheManagerConfiguration>();
            if (IsRedisNeverExpires)
                this._cacheManager = CacheFactory.FromConfiguration<object>(ConstDefine.CacheManagerRedisNeverExpiresKey, cacheManagerConfigurations.First(x => x.Name == ConstDefine.CacheManagerRedisNeverExpiresKey));
            else
                this._cacheManager = CacheFactory.FromConfiguration<object>(ConstDefine.CacheManagerDefaultKey, cacheManagerConfigurations.First(x => x.Name == ConstDefine.CacheManagerDefaultKey));
            this._cacheKey = $"{PREFIX}{context.Method.DeclaringType.Namespace}_{context.Method.Name}_{(context.Arguments[0] as ICacheKey).GetCacheKey()}";
            var cacheData = await Task.FromResult(this._cacheManager.Get<T>(this._cacheKey));
            return cacheData;
        }

        public async Task OnExecuted<T>(IAOPContext context, T result)
        {
            this._cacheManager.AddOrUpdate(_cacheKey, result, (v) => result);
            await Task.CompletedTask;
        }
    }
}
