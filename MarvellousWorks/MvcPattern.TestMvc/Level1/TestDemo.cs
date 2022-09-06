using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcPattern.Level1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcPattern.TestMvc.Level1
{
    [TestClass]
    public class TestDemo
    {
        [TestMethod]
        public void Test()
        {
            Demo demo = new Demo();
            demo.PrintData();
        }
    }
}
