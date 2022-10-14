using System.Windows.Markup;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ITest1 test = new Test();
            test.Handle("hello");
            ITest2 test2 = (ITest2)test;
            test2.Hanlde(100);
        }
    }

    public class Test : ITest1, ITest2
    {
        public void Handle(string name)
        {
            Console.WriteLine($"name={name}");
        }

        public void Hanlde(int age)
        {
            Console.WriteLine($"age={age}");
        }
    }

    public interface ITest1
    {
        void Handle(string name);
    }

    public interface ITest2
    {
        void Hanlde(int age);
    }
}