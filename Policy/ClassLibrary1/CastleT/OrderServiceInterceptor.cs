using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Castle.Test.CastleT
{
    public class OrderServiceInterceptor: StandardInterceptor
    {
        protected override void PerformProceed(IInvocation invocation)
        {

            Console.WriteLine("正在处理");
            base.PerformProceed(invocation);
        }
        protected override void PostProceed(IInvocation invocation)
        {
            Console.WriteLine("处理后");
            base.PostProceed(invocation);
        }

        protected override void PreProceed(IInvocation invocation)
        {
            Console.WriteLine("预处理");
            base.PreProceed(invocation);
        }
    }
}
