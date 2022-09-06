using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyPattern03
{
    public sealed class DisposableObjectPool<T>:DefaultObjectPool<T>,IDisposable where T:class
    {
        private volatile bool _isDisposed;
        public DisposableObjectPool(IPooledObjectPolicy<T> policy) : base(policy) { }
        public DisposableObjectPool(IPooledObjectPolicy<T> policy, int maximmRetained) : base(policy, maximmRetained)
        {
        }

        public override T Get()
        {
            if (_isDisposed)
                ThrowObjectDisposedException();
            return base.Get();
        }
        void ThrowObjectDisposedException()
        {
            throw new ObjectDisposedException(GetType().Name);
        }

        public override void Return(T obj)
        {
            if(_isDisposed || !ReturnCore(obj))
                DisposeItem(obj);
        }
        private bool ReturnCore(T obj)
        {
            bool returnedTooPool = false;
            if(!_isDefaultPolicy|| (_fastPolicy?.Return(obj) ?? _policy.Return(obj)))
            {
                if (_firstItem == null && Interlocked.CompareExchange(ref _firstItem, obj, null) == null)
                    returnedTooPool = true;
                else
                {
                    var items = _items;
                    for (var i = 0; i < items.Length && !(returnedTooPool = Interlocked.CompareExchange(ref items[i].Element, obj, null) == null); i++)
                    {
                    }
                }
            }
            return returnedTooPool;
        }
        public void Dispose()
        {
            _isDisposed = true;

        }
        private static void DisposeItem(T? item)
        {
            if (item is IDisposable disposable)
                disposable.Dispose();
        }
    }
}
