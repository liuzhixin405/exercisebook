
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace WebApi幂等性
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class IdempotentAttribute : Attribute, IFilterFactory
    {
        public bool IsReusable => false;

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            //var distributedCache = (IDistributedCache)serviceProvider.GetService(typeof(IDistributedCache));

            //var filter = new IdempotentAttributeFilter(distributedCache);
            var cache = serviceProvider.GetService<IMemoryCache>();
                var filter = new IdempotentAttributeFilter(cache);
            return filter;
        }

    }
}
