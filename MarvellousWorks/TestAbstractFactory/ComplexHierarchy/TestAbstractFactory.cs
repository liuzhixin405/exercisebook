using AbstractFactory.ComplexHierarchy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestAbstractFactory.ComplexHierarchy
{
    public class TestAbstractFactory
    {
        [Fact]
        public void Test()
        {
            IAbstractFactoryWithTypeMapper factory = new ConcreteFactoryX(); //有TypeMapper 和Create()
            AssemblyMechanism.Assembly(factory);    //绑定TypeMapper
            IProductXB productXB = factory.Create<IProductXB>();
            Assert.NotNull(productXB);
            Assert.Equal<Type>(typeof(ProductXB1), productXB.GetType());

            factory = new ConcreteFactoryY();
            AssemblyMechanism.Assembly(factory);
            IProductYC productYC = factory.Create<IProductYC>();
            Assert.NotNull(productYC);
            Assert.Equal<Type>(typeof(ProductYC1), productYC.GetType());

        }
    }
}
