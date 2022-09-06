using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObserverPattern.Classic
{
    /// <summary>
    /// 观察者类型接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IObserver<T>
    {
        void Update(SubjectBase<T> subject);
    }
    /// <summary>
    /// 目标对象抽象类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SubjectBase<T>
    {
        /// <summary>
        /// 登记需要通知的观察者
        /// </summary>
        protected IList<IObserver<T>> observers = new List<IObserver<T>>();

        protected T state;
        public virtual T State => state;

        #region + -
        public static SubjectBase<T> operator +(SubjectBase<T> subject, IObserver<T> obserer)
        {
            subject.observers.Add(obserer);
            return subject;
        }

        public static SubjectBase<T> operator -(SubjectBase<T> subject, IObserver<T> obserer)
        {
            subject.observers.Remove(obserer);
            return subject;
        } 
        #endregion
        /// <summary>
        /// 更新观察者
        /// </summary>
        public virtual void Notify()
        {
            foreach (var observer in observers)
            {
                observer.Update(this);
            }
        }
        /// <summary>
        /// 供客户程序对目标对象进行操作的方法
        /// </summary>
        /// <param name="state"></param>
        public virtual void Update(T state)
        {
            this.state = state;
            Notify(); // 触发对外通知
        }
    }
    /// <summary>
    /// 具体目标对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SubjectA<T> : SubjectBase<T>
    {

    }
    /// <summary>
    /// 具体目标对象2
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SubjectB<T> : SubjectBase<T>
    {

    }
    public class Observer<T> : IObserver<T>
    {
        public T State;
        public void Update(SubjectBase<T> subject)
        {
            this.State = subject.State;
        }
    }
}
