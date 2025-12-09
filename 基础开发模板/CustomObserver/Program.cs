using CustomObserver.Impl;

namespace CustomObserver
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IObserverFactory<Data> observerFactory = new ObserverFactory();
            observerFactory.AddObservable(new StartObserable());
            observerFactory.AddObservable(new DoneObserable());

            var opt = observerFactory.Create("gogogo");
            opt.UpdateAsync(new Data { Message= "i come on!"});
            Console.Read();
        }
    }
}
