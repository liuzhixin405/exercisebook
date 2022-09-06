using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyPattern03
{
    public interface IPooledObjectPolicy<T> where T:notnull
    {
        T Create();
        bool Return(T obj);
    }
}
