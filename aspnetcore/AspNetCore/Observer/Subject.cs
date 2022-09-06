using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace AspNetCore.Observer
{
    public class Test
    {
        public class MyAction { }
        public class MyConcrete { }
        private List<Action<MyAction, MyConcrete>> _list = new List<Action<MyAction, MyConcrete>>();

        private MyAction action = new MyAction();
        private MyConcrete concrete = new MyConcrete();

        public void AddAction()
        {
            _list.Add((a,c) => { Console.WriteLine("test"); });
        }
        public void Invoke()
        {
            foreach (var item in _list)
            {
                item(action, concrete);
            }
        }
    }

  /// <summary>
  /// 发布者
  /// </summary>
    public abstract class Subject
    {
        private ArrayList observers = new ArrayList();

        public void Attach(Observer observer)
        {
            observers.Add(observer);

        }
        public void Detach(Observer observer)
        {
            observers.Remove(observer);
        }

        public void Notify()
        {
            foreach (Observer o in observers)
            {
                o.Update();
            }
        }
    }


    public class ConcreteSubject : Subject
    {
        private string _subjectState;

        public string SubjectState
        {
            get { return _subjectState; }
            set { _subjectState = value; }
        }
    }
    /// <summary>
    /// 订阅者
    /// </summary>
    public abstract class Observer
    {
        public abstract void Update();
    }

    public class ConcreteObserver:Observer
    {
        private string _name;
        private string _observerState;
        private ConcreteSubject _subject;
        public ConcreteObserver(ConcreteSubject subject,string name)
        {
            this._subject = subject;
            this._name = name;
        }

        public override void Update()
        {
            _observerState = _subject.SubjectState;
            Console.WriteLine($"Observer {_name}'s new state is{_observerState}");
        }

        public ConcreteSubject Subject
        {
            get
            {
                return _subject;
            }
            set
            {
                _subject = value;
            }
        }

    }
}
