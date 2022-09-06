using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyPattern03
{
    public class DefaultObjectPool<T> : ObjectPool<T> where T : class
    {
        private protected readonly ObjectWrapper[] _items;
        private protected readonly IPooledObjectPolicy<T> _policy;
        private protected readonly bool _isDefaultPolicy;
        private protected T? _firstItem;

        private protected readonly PooledObjectPolicy<T>? _fastPolicy;
        public DefaultObjectPool(IPooledObjectPolicy<T> policy):this(policy,Environment.ProcessorCount *2)
        {

        }
        public DefaultObjectPool(IPooledObjectPolicy<T> policy,int maximmRetained)
        {
            _policy = policy ?? throw new ArgumentNullException(nameof(policy));
            _fastPolicy = policy as PooledObjectPolicy<T>;
            _isDefaultPolicy = isDefaultPolicy();
        }
        bool isDefaultPolicy()
        {
            var type = _policy.GetType();
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(DefaultPooledObjectPolicy<>);
        }

        public override T Get()
        {
            var item = _firstItem;
            if (item == null || Interlocked.CompareExchange(ref _firstItem, null, item) == item)
            {
                var items = _items;
                for (int i = 0; i < items.Length; i++)
                {
                    item = items[i].Element;
                    if(item!=null && Interlocked.CompareExchange(ref items[i].Element, null, item) == item)
                    {
                        return item;
                    }
                }
                item = Create();
            }
            return item;
        }
        private T Create() => _fastPolicy?.Create() ?? _policy.Create();
        public override void Return(T obj)
        {
           if(_isDefaultPolicy||(_fastPolicy?.Return(obj)?? _policy.Return(obj)))
            {
                if(_firstItem!=null || Interlocked.CompareExchange(ref _firstItem, obj, null) != null)
                {
                    var items = _items;
                    for (int i = 0; i < items.Length&&Interlocked.CompareExchange(ref items[i].Element,obj,null)!=null; i++)
                    {

                    }
                }
            }
        }
        [DebuggerDisplay("{Element}")]
        private protected struct ObjectWrapper
        {
            public T? Element;
        }
    }
}
