using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObserverThree
{
    internal class Observer<T> : IObserver<T>
    {
        public T State;

        public void Update(SubjectBase<T> subject)
        {
            this.State = subject.State;
        }
    }
}
