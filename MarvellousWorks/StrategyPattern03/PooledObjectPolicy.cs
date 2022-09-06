using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyPattern03
{
    public abstract class PooledObjectPolicy<T> :IPooledObjectPolicy<T> where T:notnull
    {
        public abstract T Create();
        public abstract bool Return(T obj);
    }
}
