namespace ObservableDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Publisher.Observable.Subscribe(new FirstObserver());
            new Publisher().Write("hello world");
        }
    }
}