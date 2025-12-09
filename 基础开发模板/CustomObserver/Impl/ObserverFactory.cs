using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomObserver.Impl
{
    internal class ObserverFactory : IObserverFactory<Data>
    {
        private readonly IList<IObservable<Data>> _observables;
        internal ObserverFactory(List<IObservable<Data>> observables=null)
        {
            _observables = observables ??new List<IObservable<Data>>();
        }
        public void AddObservable(IObservable<Data> observable)
        {
            _observables.Add(observable);
        }

        public IObserver<Data> Create(string type)
        {
            var lsitObserver = _observables.Select(x=>x.Create(type));
            return new CompositeObserver<Data>(lsitObserver);
        }

        public void RemoveObservable(IObservable<Data> observable)
        {
            _observables.Remove(observable);
        }
    }
}
