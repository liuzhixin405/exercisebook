using FactoryMethod;
using System;
using Xunit;

namespace FactoryMethodTest
{
    public class ProductCollectionTest
    {
        [Fact]
        public void Test()
        {
            ProductCollection collection = new ProductCollection();
            for (int i = 0; i < 3; i++)
            {
                collection.Insert(new ProductA());
            }
            Assert.Equal<int>(3, collection.Count);
            IProduct[] products = collection.Data;
            foreach (var item in products)
            {
                Assert.Equal("A", item.Name);
            }
        }
    }
}
