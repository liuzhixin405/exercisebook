using Castle.Test.CastleT;
using System;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            InterceptorTest.TestInterceptorByClass();
            InterceptorTest.TestInterceptorByInterface();
            Console.WriteLine("Hello World!");
        }
    }
}
