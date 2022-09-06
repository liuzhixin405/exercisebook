using System;
using System.Collections.Generic;

namespace ConsoleApp
{
    internal class Program
    {
        static void Four_Main(string[] args)
        {
            var mother = HotDaugther.Activator().Message;
            Console.WriteLine("mother="+mother);
            var create = new HotDaugther();
            var daughter = HotDaugther.Activator().Message;
            Console.WriteLine("daughter="+daughter);
            Console.WriteLine(create.Message);
            (_ = (Action<string>)((string str) => { Console.Write(str); }))("你见过这个东西吗 ");

            var bs = new
            {             
                Print = (Action<string>)((string str) => { Console.WriteLine(str); })
            };
            bs.Print(" Jack");
            Console.Read();
        }

        public Action AutoSave { get; private set; }
    }

    class CoolMother
    {
        public static Func<CoolMother> Activator { get; protected set; }
        static CoolMother()
        {
            Activator = () => new CoolMother();
        }
        public CoolMother()
        {
            Message = "I am mother";
        }
        public string Message { get; protected set; }
    }

    class HotDaugther : CoolMother
    {
        public HotDaugther()
        {
            Activator = () => new HotDaugther();
            Message = "I am daughter";
        }
    }
    internal class MybaseClass
    {
        public Action SomeAction { get; private set; }
        Stack<Action> previousActions;
       protected void AddSomeAction(Action newMethod)
        {
            previousActions.Push(newMethod);
            SomeAction = newMethod;
        }
        protected void RemoveSomeAction()
        {
            if (previousActions.Count == 0)
                return;
            SomeAction = previousActions.Pop();
        }
        public MybaseClass()
        {
            previousActions = new Stack<Action>();
            SomeAction = () =>
            {

            };
        }
    }
}
