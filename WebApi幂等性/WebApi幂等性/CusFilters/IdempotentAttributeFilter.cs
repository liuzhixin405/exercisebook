using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;

namespace WebApi幂等性
{
    public class IdempotentAttributeFilter : IActionFilter, IResultFilter
    {
        //private readonly IDistributedCache _distributedCache;
        private readonly IMemoryCache _memoryCache;
        private bool _isIdempotencyCache = false;
        const string IdempotencyKeyHeaderName = "IdempotencyKey";
        private string _idempotencyKey;
        //public IdempotentAttributeFilter(IDistributedCache distributedCache)
        //{
        //    _distributedCache = distributedCache;
        //}
        public IdempotentAttributeFilter(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            Microsoft.Extensions.Primitives.StringValues idempotencyKeys;
            context.HttpContext.Request.Headers.TryGetValue(IdempotencyKeyHeaderName, out idempotencyKeys);
            _idempotencyKey = idempotencyKeys.ToString();

            //var cacheData = _distributedCache.GetString(GetDistributedCacheKey());
            var cacheData = _memoryCache.Get(GetDistributedCacheKey()) as string;
            if (cacheData != null)
            {
                context.Result = JsonConvert.DeserializeObject<ObjectResult>(cacheData);
                _isIdempotencyCache = true;
                return;
            }
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            //已缓存
            if (_isIdempotencyCache)
            {
                return;
            }

            var contextResult = context.Result;

            DistributedCacheEntryOptions cacheOptions = new DistributedCacheEntryOptions();
            cacheOptions.AbsoluteExpirationRelativeToNow = new TimeSpan(24, 0, 0);

            //缓存:
            //_distributedCache.SetString(GetDistributedCacheKey(), JsonConvert.SerializeObject(contextResult), cacheOptions);
            _memoryCache.Set(GetDistributedCacheKey(), JsonConvert.SerializeObject(contextResult),TimeSpan.FromMinutes(1));
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
        }

        private string GetDistributedCacheKey()
        {
            return "Idempotency:" + _idempotencyKey;
        }

    }
}
