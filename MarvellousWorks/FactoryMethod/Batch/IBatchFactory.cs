using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryMethod.Batch
{
    /// <summary>
    /// 定义抽象批量产品加工工厂
    /// </summary>
    public interface IBatchFactory
    {
        /// <summary>
        /// 批量工厂的抽象工厂行为定义
        /// </summary>
        /// <param name="quantity">代加工产品的数量</param>
        /// <returns></returns>
        ProductCollection Create(int quantity);
    }

    /// <summary>
    /// 为了方便扩展提供的抽象基类
    /// </summary>
    /// <typeparam name="T">Concrete Product类型</typeparam>
    public class BatchProductFactoryBase<T> : IBatchFactory where T:IProduct,new()
    {
        /// <summary>
        /// 给一个实现
        /// </summary>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public virtual ProductCollection Create(int quantity)
        {
            if (quantity <= 0) throw new ArgumentNullException("quantity");
            ProductCollection collection = new ProductCollection();
            for (int i = 0; i < quantity; i++)
            {
                collection.Insert(new T());
            }
            return collection;
        }
    }
    /// <summary>
    /// 两个实体批量生产工厂类型
    /// </summary>
    public class BatchProductAFactory : BatchProductFactoryBase<ProductA> { }
    public class BatchProductBFactory : BatchProductFactoryBase<ProductB> { }

}
