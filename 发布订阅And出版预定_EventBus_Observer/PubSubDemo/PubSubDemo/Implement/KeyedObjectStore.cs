using PubSubDemo.BusinessEntity;
using PubSubDemo.DataEntity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSubDemo.Implement
{
    /// <summary>
    /// 测试用内存持久化机制
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class KeyedObjectStore<T> : IKeyedObjectStore<T> where T : class, IObjectWithKey
    {
        protected IDictionary<string,T> data = new Dictionary<string,T>();
        public IEnumerator GetEnumerator()
        {
            foreach (T val in data.Values)
            {
                yield return val;
            }
        }

        public virtual void Remove(string key)
        {
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            data.Remove(key);
        }

        public virtual void Save(T target)
        {
            if(target == null)  throw new ArgumentNullException("target");
            data.Add(target.Key, target);   
        }

        public virtual T Select(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            T result;
            if (data.TryGetValue(key, out result))
                return result;
            else
                return default(T);
        }
    }
}
