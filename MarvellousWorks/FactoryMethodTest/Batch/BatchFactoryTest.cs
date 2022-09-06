using FactoryMethod;
using FactoryMethod.Batch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FactoryMethodTest.Batch
{
    class ProductADecision : DecisionBase
    {
        /// <summary>
        /// 定义ProductA 的生产计划
        /// </summary>
        public ProductADecision():base(new BatchProductAFactory(), 2) { }
    }
    class ProductBDecison : DecisionBase
    {
        /// <summary>
        /// 定义PoductB 的生产计划
        /// </summary>
        public ProductBDecison():base(new BatchProductBFactory(), 3) { }
    }

    class ProductDirector : DirectorBase
    {
        public ProductDirector()
        {
            base.Insert(new ProductADecision());
            base.Insert(new ProductBDecison());
        }
    }
    class Client
    {
        private DirectorBase director = new ProductDirector();
        public IProduct[] Produce()
        {
            ProductCollection collection = new ProductCollection();
            foreach (DecisionBase decision in director.Decisions)
            {
                collection += decision.Factory.Create(decision.Quantity);
            }
            return collection.Data;
        }
    }
    public class BatchFactoryTest
    {
        [Fact]
        public void Test()
        {
            Client client = new Client();
            IProduct[] products = client.Produce();
            Assert.Equal<int>(2 + 3, products.Length);
            for (int i = 0; i < 2; i++)
            {
                Assert.Equal("A", products[i].Name);
            }
            for (int i = 2; i < 5; i++)
            {
                Assert.Equal("B", products[i].Name);
            }
        }
    }
}
