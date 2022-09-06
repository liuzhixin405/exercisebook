using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryMethod
{
    /// <summary>
    /// 装载Iproduct的容器类型
    /// </summary>
    public class ProductCollection
    {
        private IList<IProduct> data = new List<IProduct>();

        /// <summary>
        /// 对外的集合操作方法
        /// </summary>
        /// <param name="items"></param>
        public void Insert(IProduct[] items)
        {
            if (items == null || items.Length == 0) return;
            foreach (var item in items)
            {
                data.Add(item);
            }
        }
        public void Insert(IProduct item) { data.Add(item); }
        public void Remove(IProduct item)
        {
            data.Remove(item);
        }
        public void Clear() { data.Clear(); }

        /// <summary>
        /// 获取所有Iproduct内容的属性
        /// </summary>
        public IProduct[] Data
        {
            get
            {
                if (data == null || data.Count == 0) return null;
                IProduct[] result = new IProduct[data.Count];
                data.CopyTo(result, 0);
                return result;
            }
        }
        public int Count => data.Count;

        /// <summary>
        /// 重载运算符
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public static ProductCollection operator +(ProductCollection collection,IProduct[] items)
        {
            ProductCollection result = new ProductCollection();
            if (collection != null && collection.Count != 0) result.Insert(collection.Data);
            if (items != null && items.Length != 0) result.Insert(items);
            return result;
        }
        public static ProductCollection operator +(ProductCollection source, ProductCollection target)
        {
            ProductCollection result = new ProductCollection();
            if (source != null && target.Count != 0) result.Insert(source.Data);
            if (target != null && target.Count != 0) result.Insert(target.Data);
            return result;
        }

        public static ProductCollection operator -(ProductCollection collection, IProduct[] items)
        {
            ProductCollection result = new ProductCollection();
            if (collection != null && collection.Count != 0) result.Insert(collection.Data);
            if (items != null && items.Length != 0)
                foreach (var item in items)
                {
                    result.Remove(item);
                }
            return result;
        }
        public static ProductCollection operator -(ProductCollection source, ProductCollection target)
        {
            ProductCollection result = new ProductCollection();
            if (source != null && target.Count != 0) result.Insert(source.Data);
            if (target != null && target.Count != 0)
            {
                foreach (var item in target.data)
                {
                    result.Remove(item);
                }
            }
            return result;
        }
    }
}
