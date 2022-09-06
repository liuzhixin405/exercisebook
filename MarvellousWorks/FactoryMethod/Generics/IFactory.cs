using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryMethod.Generics
{
    /// <summary>
    /// 抽象的泛型工厂类型
    /// </summary>
    public interface IFactory<T>
    {
        T Create();
    }
    public abstract class FactoryBase<T> : IFactory<T> where T:new()
    {
        /// <summary>
        /// 由于批量工厂的可能应用概率比较小,因此默认实现了单个产品的工厂
        /// </summary>
        /// <returns></returns>
        public virtual T Create()
        {
            return new T();
        }
    }
    /// <summary>
    /// 生产单个产品的实体工厂
    /// </summary>
    public class ProductAFactory: FactoryBase<ProductA> { }
    public class ProductBFactory : FactoryBase<ProductB> { }
    /// <summary>
    /// 生产批量产品工厂的抽象定义
    /// </summary>
    /// <typeparam name="TCollection"></typeparam>
    /// <typeparam name="TItem"></typeparam>
    public abstract class BatchFactoryBase<TCollection, TItem> : FactoryBase<TCollection> 
        where TCollection:ProductCollection,new()
        where TItem:IProduct,new()
    {
        protected int quantity;
        public virtual int Quantity { set { this.quantity = value; } }

        public override TCollection Create()
        {
            if (quantity <= 0) throw new ArgumentException("quantity");
            TCollection collection = new TCollection();
            for(int i = 0; i < quantity; i++)
            {
                collection.Insert(new TItem());
            }
            return collection;
        }
    }
    /// <summary>
    /// 生产批量产品的实体工厂
    /// </summary>
    public class BatchProductAFactory : BatchFactoryBase<ProductCollection, ProductA> { }
    public class BatchProductBFactory : BatchFactoryBase<ProductCollection, ProductB> { }
}
