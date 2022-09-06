using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObserverThree
{
    /// <summary>
    /// 观察者
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal interface IObserver<T>
    {
        void Update(SubjectBase<T> subject);
    }
}
