using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyPattern03.Provider
{
    public class DefaultObjectPoolProvider:ObjectPoolProvider
    {
        public int MaximmRetained { get; set; } = Environment.ProcessorCount * 2;

        public override ObjectPool<T> Create<T>(IPooledObjectPolicy<T> policy)
        {
            if (policy == null) throw new ArgumentNullException(nameof(policy));
            if (typeof(IDisposable).IsAssignableFrom(typeof(T)))
                return new DisposableObjectPool<T>(policy, MaximmRetained);
            return new DefaultObjectPool<T>(policy, MaximmRetained);
        }
    }
}
