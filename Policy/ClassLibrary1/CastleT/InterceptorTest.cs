using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Castle.Test.CastleT
{
    public class InterceptorTest
    {
        public static void TestInterceptorByClass()
        {
            ProxyGenerator proxyGenerator = new ProxyGenerator();
            OrderServiceInterceptor interceptor = new OrderServiceInterceptor();

            OrderService orderService = proxyGenerator.CreateClassProxy<OrderService>(interceptor);

            orderService.MethodInterceptor();     //代理
            orderService.MethodNoInterceptor();    //非代理

        }

        public static void TestInterceptorByInterface()
        {
            ProxyGenerator proxyGenerator = new ProxyGenerator();
            OrderServiceInterceptor interceptor = new OrderServiceInterceptor();
            IOrderService orderServiceTwo = proxyGenerator.CreateInterfaceProxyWithTarget<IOrderService>(new OrderServiceTwo(), interceptor);
            orderServiceTwo.MethodInterceptor();
            orderServiceTwo.MethodNoInterceptor();
        }
    }
}
