using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObserverSeven
{
    internal class Teacher : IObservable<Message>
    {
        private readonly List<IObserver<Message>> _observers;
        public Teacher()
        {
            _observers = new List<IObserver<Message>>();
        }
        public IDisposable Subscribe(IObserver<Message> observer)
        {
            _observers.Add(observer);
            return new Unsubscribe(observer, _observers);
        }

        public void SendMessage(string message)
        {
            foreach (var observer in _observers)
            {
                observer.OnNext(new Message() { Notify = "message" });
            }
        }
        public void OnCompleted()
        {
            foreach (var observer in _observers)
            {
                observer.OnCompleted();
            }
            _observers.Clear();
        }
    }

    internal class Unsubscribe:IDisposable
    {
        private readonly IObserver<Message> _observer;
        private readonly List<IObserver<Message>> _observers;
        public Unsubscribe(IObserver<Message> observer, List<IObserver<Message>> observers)
        {
            this._observers = observers;
            this._observer = observer;
        }

        public void Dispose()
        {
            if(_observers.Contains(_observer))
                _observers.Remove(_observer);
        }
    }
}
