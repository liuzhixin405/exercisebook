using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplatePattern.temp4;

namespace TemplatePattern.Test.temp4
{
    [TestClass]
    public class TestTemplate
    {
        [TestMethod]
        public void Test()
        {
            TemplateList<int> list = new TemplateList<int>();
            list.Add(2);
            list.Add(3);
            int i = 3;
            foreach (int data in list)
            {
                Assert.AreEqual<int>(i--, data);
            }
        }
    }
}
