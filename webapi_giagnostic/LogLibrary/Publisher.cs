using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogLibrary
{
    public class Publisher : IObservable<Messager>
    {
        private readonly List<IObserver<Messager>> _observers;
        public Publisher()
        {
            _observers = new List<IObserver<Messager>>();
        }
        public IDisposable Subscribe(IObserver<Messager> observer)
        {
            _observers.Add(observer);
            return new Unsubscribe(observer, _observers);
        }
    }
    internal class Unsubscribe : IDisposable
    {
        private readonly IObserver<Messager> _observer;
        private readonly List<IObserver<Messager>> _observers;
        public Unsubscribe(IObserver<Messager> observer, List<IObserver<Messager>> observers)
        {
            this._observers = observers;
            this._observer = observer;
        }

        public void Dispose()
        {
            if (_observers.Contains(_observer))
                _observers.Remove(_observer);
        }
    }


}
