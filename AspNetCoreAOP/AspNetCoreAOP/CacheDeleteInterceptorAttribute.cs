using AspectCore.DynamicProxy;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.ObjectPool;

namespace AspNetCoreAOP
{
    public class CacheDeleteInterceptorAttribute:AbstractInterceptorAttribute
    {
        private readonly Type[] _types;
        private readonly string[] _methods;
        public CacheDeleteInterceptorAttribute(Type[] types, string[] methods)
        {
            if (types.Length != methods.Length)
            {
                throw new Exception("Types必须跟Methods数量一致");
            }
            _types = types;
            _methods = methods;
        }

        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            var cache = context.ServiceProvider.GetService<MemoryCache>();
            await next(context);
            for (int i = 0; i < _types.Length; i++)
            {
                var type = _types[i];
                var method = _methods[i];
                string key = "Methods:" + type.FullName + "." + method;
                cache.Remove(key);
            }
        }
    }
}
