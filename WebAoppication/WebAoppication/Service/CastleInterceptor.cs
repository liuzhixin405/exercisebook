using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAoppication
{
    public class CastleInterceptor : AsyncInterceptorBase
    {
        private readonly IServiceProvider _serviceProvider;
        public CastleInterceptor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private IAOPContext _aopContext;
        private List<BaseAOPAttribute> _aops;
        private async Task Befor()
        {
            foreach (var aAop in _aops)
            {
                await aAop.Befor(_aopContext);
            }
        }
        private async Task After()
        {
            foreach (var aAop in _aops)
            {
                await aAop.After(_aopContext);
            }
        }
        private void Init(IInvocation invocation)
        {
            _aopContext = new CastleAOPContext(invocation, _serviceProvider);

            _aops = invocation.MethodInvocationTarget.GetCustomAttributes(typeof(BaseAOPAttribute), true)
                .Concat(invocation.InvocationTarget.GetType().GetCustomAttributes(typeof(BaseAOPAttribute), true))
                .Select(x => (BaseAOPAttribute)x)
                .ToList();
        }

        protected override async Task InterceptAsync(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task> proceed)
        {
            Init(invocation);

            await Befor();
            await proceed(invocation, proceedInfo);
            await After();
        }

        protected override async Task<TResult> InterceptAsync<TResult>(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed)
        {
            Init(invocation);

            TResult result = default(TResult);
            var cacheAop = _aops?.FirstOrDefault(x => x is CacheDataAttribute) as CacheDataAttribute;
            if (cacheAop != null)
                result = await cacheAop.OnExecuting<TResult>(_aopContext);
            if (result != null)
                return result;

            await Befor();
            result = await proceed(invocation, proceedInfo);
            await After();

            if (cacheAop != null)
                await cacheAop.OnExecuted<TResult>(_aopContext, result);

            return result;
        }
    }
}
