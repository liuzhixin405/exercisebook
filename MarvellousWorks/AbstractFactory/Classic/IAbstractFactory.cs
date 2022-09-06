using AbstractFactory.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractFactory.Classic
{
    /// <summary>
    /// 实体工厂
    /// </summary>
    public class ConcreteFactory1 : IAbstractFactory
    {
        /// <summary>
        /// IAbstractFactory Members
        /// </summary>
        /// <returns></returns>
        public virtual IProductA CreateProductA()
        {
            return new ProductA1();
        }

        public virtual IProductB CreateProductB()
        {
            return new ProductB1();
        }

        public virtual IProductC CreateProductC()
        {
            return new ProductC1();
        }
    }

    public class ConcreteFactory2 : IAbstractFactory
    {
        public virtual IProductA CreateProductA()
        {
            return new ProductA2Y();
        }

        public virtual IProductB CreateProductB()
        {
            return new ProductB2();
        }
        public virtual IProductC CreateProductC()
        {
            return new ProductC2();
        }
    }
}
