using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomObserver.Impl
{
    internal class CompositeObserver<T>:IObserver<T>
    {
        private readonly IEnumerable<IObserver<T>> _observers;
        public CompositeObserver(IEnumerable<IObserver<T>> observers)
        {
            _observers = observers;
        }
        public async Task UpdateAsync(T message)
        {
           _observers.Select(async observer =>await observer.UpdateAsync(message)).ToList();
        }
    }
}
