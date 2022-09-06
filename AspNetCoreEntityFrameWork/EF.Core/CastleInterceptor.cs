using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF.Core
{
    public class CastleInterceptor : AsyncInterceptorBase
    {
        #region ctor
        private readonly IServiceProvider _serviceProvider;
        public CastleInterceptor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private IAOPContext _aopContext;
        private List<BaseAOP> _aops;

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
        #endregion
        private void Init(IInvocation invocation)
        {
            _aopContext = new CastleAOPContext(invocation, _serviceProvider);

            _aops = invocation.MethodInvocationTarget.GetCustomAttributes(typeof(BaseAOP), true)
                .Concat(invocation.InvocationTarget.GetType().GetCustomAttributes(typeof(BaseAOP), true))
                .Select(x => (BaseAOP)x)
                .ToList();
        }


        protected override async Task InterceptAsync(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task> proceed)
        {
            Init(invocation);
            await Befor();
            await proceed(invocation, proceedInfo);
            await After();
        }

        /// <summary>
        /// 带返回值缓存结果
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="invocation"></param>
        /// <param name="proceedInfo"></param>
        /// <param name="proceed"></param>
        /// <returns></returns>
        protected async override Task<TResult> InterceptAsync<TResult>(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed)
        {
            Init(invocation);
            TResult result = default(TResult); //返回值类型 bool会报错 
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
