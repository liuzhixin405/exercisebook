using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplatePattern.temp1;

namespace TemplatePattern.Test.temp1
{
    [TestClass]
    public class TestTemplate
    {
        [TestMethod]
        public void Test()
        {
            IAbstract i1 = new ArrayData();
            Assert.IsTrue(Math.Abs(i1.Average - 2.2) <= 0.001);
            IAbstract i2 = new ListData();
            Assert.IsTrue(Math.Abs(i1.Average - i2.Average) <= 0.001);
        }
    }
}
