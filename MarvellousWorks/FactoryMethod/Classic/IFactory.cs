using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryMethod.Classic
{
    /// <summary>
    /// 抽象的工厂类型特性描述
    /// </summary>
    public interface IFactory
    {
        IProduct Create(); //每个工厂锁需要具有的工厂方法--创建产品
    }
    /// <summary>
    /// 实体工厂类型
    /// </summary>
    public class FactoryA : IFactory
    {
        public IProduct Create()
        {
            return new ProductA();
        }
    }

    public class FactoryB : IFactory
    {
        public IProduct Create()
        {
            return new ProductB();
        }
    }
}
