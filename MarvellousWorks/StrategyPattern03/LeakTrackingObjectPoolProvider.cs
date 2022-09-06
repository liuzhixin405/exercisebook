using StrategyPattern03.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyPattern03
{
    public class LeakTrackingObjectPoolProvider:ObjectPoolProvider
    {
        private readonly ObjectPoolProvider _inner;
        public LeakTrackingObjectPoolProvider(ObjectPoolProvider inner)
        {
            if (inner == null)
            {
                throw new ArgumentNullException(nameof(inner));
            }

            _inner = inner;
        }
        public override ObjectPool<T> Create<T>(IPooledObjectPolicy<T> policy)
        {
            var inner = _inner.Create<T>(policy);
            return new LeakTrackingObjectPool<T>(inner);
        }
    }
}
