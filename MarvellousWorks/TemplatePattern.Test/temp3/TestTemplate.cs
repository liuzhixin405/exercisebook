using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplatePattern.temp3;

namespace TemplatePattern.Test.temp3
{
    [TestClass]
    public class TestTemplate
    {
        [TestMethod]
        public void Test()
        {
            Counter counter = new Counter();
            counter.Changed += (object sender,CounterEventArgs args)=>Assert.AreEqual<int>(1, args.Value);
            counter.Add();
        }
    }
}
