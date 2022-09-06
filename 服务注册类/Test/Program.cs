using Autofac;
using System;
using 服务注册类;

namespace Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<TestDomain>().As<ITestDomain>().SingleInstance();
            ServiceLocator.Current = builder.Build();
       
            TestService testService = new TestService();
            var result = testService.GetValue();
            Console.WriteLine(result);
        }
    }
}
