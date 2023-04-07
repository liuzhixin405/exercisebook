using Microsoft.Extensions.DiagnosticAdapter;
using System.Data;
using System.Diagnostics;

namespace DiagnosticLog
{
    #region one
    //internal class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        DiagnosticListener.AllListeners.Subscribe(new Observer<DiagnosticListener>(listner =>
    //        {
    //            if(listner.Name== "microsoft-data-sqlclient")
    //            {
    //                listner.Subscribe(new Observer<KeyValuePair<String, object>>(eventData =>
    //                {

    //                    Console.WriteLine($"EventName:{eventData.Key}");
    //                    dynamic payload = eventData.Value;

    //                    Console.WriteLine($"commandType:{payload.CommandType}");
    //                    Console.WriteLine($"commandText:{payload.CommandText}");
    //                }));
    //            }
    //        }));
    //        var source = new DiagnosticListener("microsoft-data-sqlclient");
    //        if (source.IsEnabled("CommandExecution"))
    //        {
    //            source.Write("CommandExecution", new { CommandType = CommandType.Text, CommandText = "Select * from Test" });
    //        }
    //        Console.Read();
    //    }
    //}

    public class Observer<T> : IObserver<T>
    {
        public void OnCompleted()
        {

        }

        public void OnError(Exception error)
        {

        }

        public void OnNext(T value)
        {
            _onNext(value);
        }

        private Action<T> _onNext;
        public Observer(Action<T> onNext)
        {
            _onNext = onNext;
        }
    }
    #endregion



    #region two
    internal class App
    {
        static void Main(string[] args)
        {
            DiagnosticListener.AllListeners.Subscribe(new Observer<DiagnosticListener>(listener =>
            {
                if (listener.Name == "microsoft-data-sqlclient")
                {
                    listener.SubscribeWithAdapter(new DatabaseSourceCollector());
                }
            }));

            var source = new DiagnosticListener("microsoft-data-sqlclient");
            if (source.IsEnabled("CommandExecution"))
            {
                source.Write("CommandExecution", new { CommandType = CommandType.Text, CommandText = "Select * from Test" });
            }
            Console.Read();
        }
    }

    public class DatabaseSourceCollector
    {
        [DiagnosticName("CommandExecution")]
        public void OnCommandExecute(CommandType commandType, string commandText)
        {
            Console.WriteLine($"Event Name: CommandExecution");
            Console.WriteLine($"commandType:{commandType}");
            Console.WriteLine($"commandText:{commandText}");
        }
    }
    #endregion

}

