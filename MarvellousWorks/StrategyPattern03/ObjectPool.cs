using StrategyPattern03.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyPattern03
{
    public abstract class ObjectPool<T> where T:class
    {
        public abstract T Get();
        public abstract void Return(T obj);
    }

    public abstract class ObjectPool
    {
        public static ObjectPool<T> Create<T>(IPooledObjectPolicy<T>? policy =null) where T : class, new()
        {
          var provider = new DefaultObjectPoolProvider();
            return provider.Create(policy ?? new DefaultPooledObjectPolicy<T>());
        }

    }
}
