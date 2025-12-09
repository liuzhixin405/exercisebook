using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomObserver.Impl
{
    internal abstract class AbsObserver<T> : IObserver<T>
    {
        protected readonly string _type;
        protected AbsObserver(string typee)
        {
            _type = typee;
        }
        public abstract Task UpdateAsync(T data);
        
    }
}
