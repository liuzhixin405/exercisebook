using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObserverThree
{
    /// <summary>
    /// 可观察者(发出通知的对象)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal abstract class SubjectBase<T>
    {
        protected IList<IObserver<T>> observers = new List<IObserver<T>>();
        protected T state;
        public virtual T State => state;

        public static SubjectBase<T> operator +(SubjectBase<T> subject,IObserver<T> observer)
        {
            subject.observers.Add(observer);
            return subject;
        }
        public static SubjectBase<T> operator -(SubjectBase<T> subject,IObserver<T> observer)
        {
            subject.observers.Remove(observer);
            return subject;
        }

        public virtual void Notify()
        {
            foreach (var observer in observers)
            {
                observer.Update(this);
            }
        }

        public virtual void Update(T state)
        {
            this.state = state;
            Notify();
        }
    }
}
