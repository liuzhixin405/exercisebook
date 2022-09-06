using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Lzx.IOCDI.Framework.CustomAOP
{
    public static class SelfObjectExtend
    {
        public static object AOP(this object t,Type interfaceType)
        {
            ProxyGenerator generator = new ProxyGenerator();
            IOCInterceptor interceptor = new IOCInterceptor();
            t = generator.CreateInterfaceProxyWithTarget(interfaceType, t, interceptor);
            return t;
        }
    }

    public class IOCInterceptor : StandardInterceptor
    {
        protected override void PostProceed(IInvocation invocation)
        {
            base.PostProceed(invocation);
        }
        protected override void PerformProceed(IInvocation invocation)
        {
            MethodInfo method = invocation.Method;
            Action action = () => base.PerformProceed(invocation);
            if (method.IsDefined(typeof(BaseInterceptorAttribute), true))
            {
                foreach (var attribute in method.GetCustomAttributes<BaseInterceptorAttribute>().ToArray().Reverse())
                {
                    action = attribute.Do(invocation, action);
                }
            }

            action.Invoke();


        }
    }

    public abstract class BaseInterceptorAttribute : Attribute
    {
        public abstract Action Do(IInvocation invocation, Action action);
    }

    public class LogBeforeAttribute : BaseInterceptorAttribute
    {
        public override Action Do(IInvocation invocation, Action action)
        {
            return () =>
            {
                Console.WriteLine($"This is LogBeforeAttribute1 {invocation.Method.Name} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}");
                //执行真逻辑
                action.Invoke();
                //写个日志---参数检查...
                Console.WriteLine($"This is LogBeforeAttribute2 {invocation.Method.Name} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}");

            };
        }
    }
    public class LogAfterAttribute : BaseInterceptorAttribute
    {
        public override Action Do(IInvocation invocation, Action action)
        {
            return () =>
            {
                Console.WriteLine($"This is LogAfterAttribute1  {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}");
                //去执行真实逻辑
                action.Invoke();
                Console.WriteLine($"This is LogAfterAttribute2  {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}");
            };
        }
    }

    public class MonitorAttribute : BaseInterceptorAttribute
    {
        public override Action Do(IInvocation invocation, Action action)
        {
            return () =>
            {
                Stopwatch stopwatch = new Stopwatch();
                Console.WriteLine("this is monitorAttribute 1");
                stopwatch.Start();
                action.Invoke();

                stopwatch.Stop();

                Console.WriteLine($"本次方法花费时间{stopwatch.ElapsedMilliseconds}ms");
                Console.WriteLine("This is MonitorAttribute 2");
            };
        }
    }

    public class LoginAttribute : BaseInterceptorAttribute
    {
        public override Action Do(IInvocation invocation, Action action)
        {
            return () =>
            {
                Console.WriteLine("This is LoginAttribute 1");
                action.Invoke();
                Console.WriteLine("This is LoginAttribute 2");
            };
        }
    }

}
