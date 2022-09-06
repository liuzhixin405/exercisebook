using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public interface ITestDomain
    {
        string GetValue();
    }

    public class TestDomain : ITestDomain
    {
        public string GetValue()
        {
            return "Hello World";
        }
    } 
}
