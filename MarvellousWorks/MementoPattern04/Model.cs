using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MementoPattern04
{
    public interface IState
    {
    }

    public interface IPersistenceStore<T> where T : IState
    {
        void Store(string originatorId, int version, T target);
        T Find(string originatorId, int version);
    }
    public abstract class OriginatorBase<T> where T : IState
    {
        protected T state;
        protected string key;
        public OriginatorBase()
        {
            key = new Guid().ToString();
        }
        protected IPersistenceStore<T> store;
        public virtual void SaveCheckpoint(int version)
        {
            store.Store(key, version, state);
        }
        public virtual void Undo(int version)
        {
            state = store.Find(key, version);
        }
    }
    public class MementoPersistenceStore<T> : IPersistenceStore<T> where T : IState
    {
        private static IDictionary<KeyValuePair<string,int>,T> map = new Dictionary<KeyValuePair<string,int>,T>();
        public T Find(string originatorId, int version)
        {
            if(map.ContainsKey(new KeyValuePair<string, int>(originatorId, version)))
            {
                return map[new KeyValuePair<string,int>(originatorId, version)];
            }
            return default(T);
        }

        public void Store(string originatorId, int version, T target)
        {
            var kv = new KeyValuePair<string, int>(originatorId, version);
            if (map.ContainsKey(kv))
                map[kv] = target;
            else
                map.Add(kv, target);
        }
    }
    public struct Position : IState
    {
        public int X;
        public int Y;
    }
    public class Originator : OriginatorBase<Position>
    {
        public Originator()
        {
            store = new MementoPersistenceStore<Position>();
        }

        public void UpdateX(int x)
        {
            base.state.X = x;
        }
        public void IncreaseY()
        {
            base.state.Y++;
        }
        public void DecreaseX()
        {
            base.state.X--;
        }
        public Position Current { get => base.state; }
    }
}
