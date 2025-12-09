using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomObserver.Impl
{
    internal class StartObserable : IObservable<Data>
    {
        public IObserver<Data> Create(string type)
        {
            return new StartObserver(type);
        }
    }
}
