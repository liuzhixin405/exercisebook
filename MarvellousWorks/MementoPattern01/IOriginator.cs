using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MementoPattern01
{
    public interface IOriginator<T,M> where T:IState where M :IMemento<T>,new()
    {
        IMemento<T> Memento { get; set; }
    }
    public abstract class OriginatorBase<T,M>:IOriginator<T,M> where T : IState where M :IMemento<T>,new()
    {
        protected T state;
        public virtual IMemento<T> Memento { get
            {
                M m = new M();
                m.State = this.state;
                return m;
            }
            set
            {
                if (value == null) throw new ArgumentNullException();
                this.state = value.State;
            }
        }
    }
}
