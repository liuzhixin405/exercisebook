using FactoryMethod.DelegateFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FactoryMethodTest.Delegating
{
    public class TestDelegateFactory
    {
        [Fact]
        public void Test()
        {
            IFactory<CalculateHandler> factory = new CalculateHandlerFactory();
            CalculateHandler handler = factory.Create();
            Assert.Equal<int>(1 + 2 + 3, handler(1, 2, 3));
        }
    }
}
