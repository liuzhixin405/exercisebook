using Castle.DynamicProxy;
using Polly;
using StandardLibrary.AttributeExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace StandardLibrary.Utility
{
    public class CustomInterceptor:StandardInterceptor
    {
        protected override void PerformProceed(IInvocation invocation)
        {
            //处理中
            base.PerformProceed(invocation);
        }
        protected override void PostProceed(IInvocation invocation)
        {
            //处理后
            base.PostProceed(invocation);
        }

        /// <summary>
        /// 预处理
        /// </summary>
        /// <param name="invocation"></param>
        protected override void PreProceed(IInvocation invocation)
        {
            ISyncPolicy policy = null;
            Action<ISyncPolicy> action = po =>
            {
                policy.Execute(() => {
                    base.PreProceed(invocation);
                });
            };

            if (invocation.Method.IsDefined(typeof(PollyRetryAttribute), true))
            {
                var attributeList = invocation.Method.GetCustomAttributes<PollyRetryAttribute>().Reverse().OrderByDescending(t => t.Order);

                foreach (var attribute in attributeList)
                {
                    action = attribute.Do(action);
                }
            }

            action(policy);
        }
    }
}
