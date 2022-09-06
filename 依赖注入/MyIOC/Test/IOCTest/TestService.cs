using System;
using System.Collections.Generic;
using System.Text;

namespace Test.IOCTest
{
    public class TestService : ITestService
    {
        public void Invoke()
        {
            Console.WriteLine("Invoke");
        }
    }
}
