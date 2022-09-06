using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IObservableDemo.Temp
{
    internal class TemperatureReporter : IObserver<Temperature>
    {
        private IDisposable unsubscriber;
        private bool first = true;
        private Temperature last;
        public void OnCompleted()
        {
            Console.WriteLine("Additional temperature data will not be transmitted.");
        }

        public virtual void Subscribe(IObservable<Temperature> observable)
        {
            unsubscriber = observable.Subscribe(this);
        }

        public virtual void OnError(Exception error)
        {
            Console.Write(error.ToString());
        }

        public virtual void OnNext(Temperature value)
        {
            Console.WriteLine("The temperature is {0}°C at {1:g}", value.Degrees, value.Date);
            if (first)
            {
                last = value;
                first = false;
            }
            else
            {
                Console.WriteLine("   Change: {0}° in {1:g}", value.Degrees - last.Degrees,
                                                              value.Date.ToUniversalTime() - last.Date.ToUniversalTime());
            }
        }
        public virtual void Unsubscribe()
        {
            unsubscriber?.Dispose();
        }
    }
}
