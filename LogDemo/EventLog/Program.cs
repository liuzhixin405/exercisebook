using System.Data;
using System.Diagnostics.Tracing;

namespace EventLog
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //DatabaseSource.Instance.OnCommandExecute(commandType: CommandType.Text, "select * from product");
            //Console.Read();
            var listener = new DatabaseSourceListener();
            DatabaseSource.Instance.OnCommandExecute(commandType: CommandType.Text, "select * from product");
            Console.Read();
        }
    }

    [EventSource(Name ="microsoft-data-sqlclient")]
    public sealed class DatabaseSource : EventSource
    {
        public static readonly DatabaseSource Instance = new DatabaseSource();
        private DatabaseSource() { }
        [Event(1)]
        public void OnCommandExecute(CommandType commandType,string commandText)
        {
            WriteEvent(1,commandType, commandText);
        }
    }

    public class DatabaseSourceListener : EventListener
    {
        protected override void OnEventSourceCreated(EventSource eventSource)
        {
            if(eventSource.Name== "microsoft-data-sqlclient")
            EnableEvents(eventSource,EventLevel.LogAlways);
        }
        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            Console.WriteLine($"EventID:{eventData.EventId}");
            Console.WriteLine($"EventName:{eventData.EventName}");
            Console.WriteLine($"Playload");
            var index = 0;
            foreach (var payloadName in eventData.PayloadNames)
            {
                Console.WriteLine($"\t{payloadName}:{eventData.Payload[index++]}");
            }
            base.OnEventWritten(eventData);
        }
    }
}