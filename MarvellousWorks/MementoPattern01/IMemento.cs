using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MementoPattern01
{
    public interface IMemento<T> where T:IState
    {
        T State { get; set; }
    }

    public abstract class MementoBase<T>: IMemento<T> where T : IState
    {
        protected T state;
        public T State { get { return state; } set { state = value; } }
    }

}
