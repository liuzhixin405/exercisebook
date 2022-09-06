using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObserverSeven
{
    internal abstract class Student : IObserver<Message>
    {
        private
            string name;
        public Student(string name)
        {
            this.name = name;
        }
        private IDisposable _unsubscribe;
        public virtual void OnCompleted()
        {
            Console.WriteLine("放学了...");
        }

        public virtual void OnError(Exception error)
        {
            Console.WriteLine("生病了...");
        }

        public virtual void OnNext(Message value)
        {
            Console.WriteLine($"大家好: 我是 {name} -_- ");
            Console.WriteLine($"老师说:{value.Notify}");
        }

        public virtual void Subscribe(IObservable<Message> obserable)
        {
            if (obserable != null)
                _unsubscribe = obserable.Subscribe(this);
        }
    }
}
