using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObservableDemo
{
    internal class FirstObserver : IObserver<Publisher>
    {
        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(Publisher value)
        {
            value.Subscribe(new Listener());
        }
    }
}
