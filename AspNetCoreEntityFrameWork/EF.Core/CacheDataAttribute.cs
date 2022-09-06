using CacheManager.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF.Core
{
    public class CacheDataAttribute: BaseAOP
    {
        private const string PREFIX = "_busAOPCacheKey_";
        //private ICacheManager<object> _cacheManager = null;
        private string _cacheKey = "Users";
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
            //if (IsRedisNeverExpires)
            //    this._cacheManager = CacheFactory.FromConfiguration<object>("", cacheManagerConfigurations.First(x => x.Name == ""));
            //else
            //    this._cacheManager = CacheFactory.FromConfiguration<object>("", cacheManagerConfigurations.First(x => x.Name == ""));
            
            this._cacheKey = $"{PREFIX}{context.Method.DeclaringType.Namespace}_{context.Method.Name}_{(context.Arguments[0] as ICacheKey).GetCacheKey()}";
            //var cacheData = await Task.FromResult(this._cacheManager.Get<T>(this._cacheKey));
            await Task.CompletedTask;
            return CacheManager.Default.Get<T>(_cacheKey);          //返回值
            //return cacheData;

        }

        public async Task OnExecuted<T>(IAOPContext context, T result)
        {
            //this._cacheManager.AddOrUpdate(_cacheKey, result, (v) => result);
            CacheManager.Default.Set_AbsoluteExpire<T>(_cacheKey,result,TimeSpan.FromMinutes(10));    //缓存十分钟
            await Task.CompletedTask;
        }
    }
}
