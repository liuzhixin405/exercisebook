using FactoryMethod;
using FactoryMethod.Classic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FactoryMethodTest.Classic
{
    public class Client
    {
        private IFactory factory;

        public Client(IFactory factory)
        {
            if (factory == null) throw new ArgumentNullException("factory");
            this.factory = factory;
        }
        public IProduct GetProduct() => factory.Create();
    }

    public class TestClient
    {
        [Fact]
        public void Test()
        {
            IFactory factory = (new Assembler()).Create<IFactory>();
            Client client = new Client(factory);
            IProduct product = client.GetProduct();

            Assert.Equal("A", product.Name);
        }
    }
}
