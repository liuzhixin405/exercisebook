using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplatePattern.temp2;

namespace TemplatePattern.Test.temp2
{
    [TestClass]
    public class TestTemplate
    {
        [TestMethod]
        public void Test()
        {
            ITransform transform = new DataBroker();
            string data = "1X@X";
            Assert.AreEqual("1Y@Y", transform.Transform(data));

            ISetter setter = new DataBroker();
            data = "H:888";
            Assert.AreEqual<string>("H:888:T", setter.Append(data));
        }
    }
}
