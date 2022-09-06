using System;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }


    }

    internal interface IKeyedObject<T>
    {
        T Get(string key);
    }

    internal class KeyedObject : IKeyedObject<string>
    {
        public string Get(string key)
        {
            throw new NotImplementedException();
        }
    }
    public abstract class BaseDbStore<T>
    {
        protected abstract IKeyedObject<T> KeyedObject { get; }
    }

    public class DbStore<string> : BaseDbStore<string>
    {
        public override IKeyedObject<string> KeyedObject => new KeyedObject<string>();
    }
}
