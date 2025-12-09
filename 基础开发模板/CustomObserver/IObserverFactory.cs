using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomObserver
{
    internal interface IObserverFactory<T>
    {
        IObserver<T> Create(string type);
        void AddObservable(IObservable<T> observable);
        void RemoveObservable(IObservable<T> observable);
    }
}
