using AspectCore.DynamicProxy;
using AspectCore.DynamicProxy.Parameters;
using Microsoft.Extensions.Caching.Memory;

namespace AspNetCoreAOP
{
    public class CacheInterceptorAttribute : AbstractInterceptorAttribute
    {
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            bool isAsync = context.IsAsync();
            var methodReturnType = context.GetReturnParameter().Type;
            if(methodReturnType==typeof(void)|| methodReturnType==typeof(Task) || methodReturnType == typeof(ValueTask))
            {
                await next(context);
                return;
            }
            var returnType = methodReturnType;
            if (isAsync)
            {
                returnType = returnType.GenericTypeArguments.FirstOrDefault();
            }
            //string param = GetParaName(context.Parameters); //获取方法的参数名,
            string key = $"Methods:{context.ImplementationMethod.DeclaringType.FullName}.{context.ImplementationMethod.Name}";//获取方法名称，也就是缓存key值
            var cache = context.ServiceProvider.GetService<MemoryCache>(); //可以使用自定义的redis或者其他缓存
            if (cache.Get(key) != null)
            {
                //反射获取缓存值
                var value = typeof(MemoryCache).GetMethod("MemoryCache.Get").MakeGenericMethod(returnType).Invoke(cache, new[] {
                    key
                    //, param 
                });
                if (isAsync)
                {

                    //判断是Task还是ValueTask
                    if (methodReturnType == typeof(Task<>).MakeGenericType(returnType))
                    {
                        //反射获取Task<>类型的返回值，相当于Task.FromResult(value)
                        context.ReturnValue = typeof(Task).GetMethod(nameof(Task.FromResult)).MakeGenericMethod(returnType).Invoke(null, new[] { value });
                    }
                    else if (methodReturnType == typeof(ValueTask<>).MakeGenericType(returnType))
                    {
                        //反射构建ValueTask<>类型的返回值，相当于new ValueTask(value)
                        context.ReturnValue = Activator.CreateInstance(typeof(ValueTask<>).MakeGenericType(returnType), value);
                    }
                }
                else
                {
                    context.ReturnValue = value;
                }
                return;
            }
            await next(context);
            object returnValue;
            if (isAsync)
            {
                returnValue = await context.UnwrapAsyncReturnValue();
                //反射获取异步结果的值，相当于(context.ReturnValue as Task<>).Result
                //returnValue = typeof(Task<>).MakeGenericType(returnType).GetProperty(nameof(Task<object>.Result)).GetValue(context.ReturnValue);

            }
            else
            {
                returnValue = context.ReturnValue;
            }
            cache.Set(key
                //, param
                , returnValue);
            if(ExpireSeconds > 0)
            {
                cache.Set(key, TimeSpan.FromSeconds(ExpireSeconds));//设置key的过期时间
            }
        }

        //private string GetParaName(object[] parameters)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// 缓存秒数
        /// </summary>
        public int ExpireSeconds { get; set; }
    }
}
