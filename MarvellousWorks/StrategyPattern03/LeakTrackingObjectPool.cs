using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace StrategyPattern03
{
    public class LeakTrackingObjectPool<T> : ObjectPool<T> where T : class
    {
        private readonly ConditionalWeakTable<T, Tracker> _tracks = new ConditionalWeakTable<T, Tracker>();
        private readonly ObjectPool<T> _inner;
        public LeakTrackingObjectPool(ObjectPool<T> inner)
        {
            _inner = inner;
        }
        public override T Get()
        {
            var value = _inner.Get();
            _tracks.Add(value, new Tracker());
            return value;
        }

        public override void Return(T obj)
        {
            if(_tracks.TryGetValue(obj,out var tracker))
            {
                _tracks.Remove(obj);
                tracker.Dispose();
            }
            _inner.Return(obj);
        }
 
    private class Tracker : IDisposable
    {
        private readonly string _stack;
        private bool _disposed;
        public Tracker()
        {
            _stack = Environment.StackTrace;
        }

        public void Dispose()
        {
            _disposed = true;
            GC.SuppressFinalize(this);
        }
        ~Tracker()
        {
            if(!_disposed && !Environment.HasShutdownStarted)
            {
                Debug.Fail($"{typeof(T).Name} was leaked. Created at: {Environment.NewLine}{_stack}");
            }
        }
    }

    }
}
