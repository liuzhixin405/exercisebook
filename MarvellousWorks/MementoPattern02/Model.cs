using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MementoPattern02
{
    public interface IState { }
    public interface IMemento<T> where T : IState { T State { get; set; } }
    public abstract class MementiBase<T> where T : IState
    {
        private T state;
        protected T State { get => state; set => state = value; }
    }

    public interface IOriginator<T> where T : IState
    {
        IMemento<T> Memento { get; set; }
    }

    public abstract class OriginatorBase<T> where T : IState
    {
        protected T state;
        protected class InternalMemento<T> : IMemento<T> where T : IState
        {
            private T state;
            public T State { get => state; set => state = value; }
        }
        protected virtual IMemento<T> CreateMemento()
        {
            IMemento<T> m = new InternalMemento<T>();
            m.State = this.state;
            return m;
        }
        private IMemento<T> m;

        public virtual void SaveCheckpoint()
        {
            m = CreateMemento();
        }
        public virtual void Undo()
        {
            if (m == null) return;
            state = m.State;
        }
    }
    
    public struct Position:IState
    {
        public int X;
        public int Y;
    }

    public class Memento : MementiBase<Position> { }
    public class Originator : OriginatorBase<Position>
    {
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
