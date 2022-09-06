using System.Diagnostics;

namespace Mediator02
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Fax<int> fax = new Fax<int>();
            Bar<int> bar = new Bar<int>();
            Baz<int> baz = new Baz<int>();
            fax.EventHandler += bar.DataChange;
            fax.EventHandler += baz.DataChange;
            fax.Data = 12;
            Trace.WriteLine($"bar.data={bar.Data},baz.data={baz.Data}");
            fax.Data = 34;
            Trace.WriteLine($"bar.data={bar.Data},baz.data={baz.Data}");
        }
    }
    public class DataEventArgs<T>
    {
        T data;
        public DataEventArgs(T data)
        {
            this.Data = data;
        }

        public T Data { get => data; set => data = value; }
    }
    public class Fax<T>
    {
        public EventHandler<DataEventArgs<T>> EventHandler;
        T data;
        public T Data { get => data; set { data = value; if (EventHandler != null) EventHandler(this, new DataEventArgs<T>(data)); } }
    }

    public abstract class ColleagueBase<T>
    {
        protected T data;

        public virtual T Data { get => data; set => data = value; }
        public virtual void DataChange(object sender, DataEventArgs<T> dataEventArgs)
        {
            this.data = dataEventArgs.Data;
        }
    }
    public class Bar<T> : ColleagueBase<T> { }
    public class Baz<T> : ColleagueBase<T> { }
}