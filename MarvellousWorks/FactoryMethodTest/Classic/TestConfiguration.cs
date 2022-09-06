using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FactoryMethodTest.Classic
{
    public class TestConfiguration
    {
        private const string SectionName = "marvellousWorks.practicalPattern.factoryMethod.customFactories";
        [Fact]
        public void Test()
        {
            NameValueCollection collection = (NameValueCollection)ConfigurationSettings.GetConfig(SectionName);
            string typeName = collection["FactoryMethod.Classic.IFactory,FactoryMethod"];
            Type type = Type.GetType(typeName);
            Assert.NotNull(type);
        }
    }
}
