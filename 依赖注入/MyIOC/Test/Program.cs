using Lzx.IOCDI.Framework.CustomContainer;
using System;
using Test.IOCTest;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            SelfContainer container = new SelfContainer();
            container.Register<ITestService, TestService>(lifetimeType:LifetimeType.Singleton);

            ITestService test = container.Resolve<ITestService>();
            test.Invoke();
            Console.WriteLine("Hello World!");
        }
    }
}
