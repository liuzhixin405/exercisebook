using System.Diagnostics;

namespace Log001Demo
{
    internal class Program
    {
        static void Main(string[] args)
        {
           var source = new TraceSource("xxx",SourceLevels.All);
            source.Listeners.Add(new ConsoleTraceListener());

            var eventTypes = (TraceEventType[])Enum.GetValues(typeof(TraceEventType));
            var eventId = 1;
            Array.ForEach(eventTypes, it => source.TraceEvent(it, eventId++,$"This is a {it} message."));
            Console.Read();
        }

    }

    public class ConsoleTeraceListener : TraceListener
    {
        public override void Write(string? message)
        {
            Console.Write(message);
        }

        public override void WriteLine(string? message)
        {
            Console.WriteLine(message);
        }
    }
}