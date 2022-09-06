using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IObservableDemo.Temp
{
    internal class TemperatureMonitor : IObservable<Temperature>
    {
        readonly List<IObserver<Temperature>> observers;
        public TemperatureMonitor()
        {
            this.observers = new List<IObserver<Temperature>>();
        }
        public IDisposable Subscribe(IObserver<Temperature> observer)
        {
            if(!observers.Contains(observer))
                observers.Add(observer);
            return new Unsubscriber(observers, observer);
        }

        public async Task GetTemperature()
        {
            Nullable<Decimal>[] temps = {14.6m, 14.65m, 14.7m, 14.9m, 14.9m, 15.2m, 15.25m, 15.2m,
                                   15.4m, 15.45m, null };
            Nullable<Decimal> previous = null;
            bool start = true;
            foreach(var temp in temps)
            {
                await Task.Delay(2500);
                if (temp.HasValue)
                {
                    if(start||(Math.Abs(temp.Value-previous.Value) >= 0.1m))
                    {
                        Temperature temperature = new(temp.Value, DateTime.Now);
                        foreach (var observer in observers)
                        {
                            observer.OnNext(temperature);
                        }
                        previous = temp;
                        if (start) start = false;
                    }
                    else
                    {
                        foreach (var observer in observers.ToArray())
                        {
                            if (observer != null) observer.OnCompleted();
                            observers.Clear();
                            break;
                        }
                    }
                }
            }
        }

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<Temperature>> _observers;
            private IObserver<Temperature> _observer;
            public Unsubscriber(List<IObserver<Temperature>> observers, IObserver<Temperature> observer)
            {
                _observers = observers;
                _observer = observer;
            }
            public void Dispose()
            {
                if(_observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }
    }
}
