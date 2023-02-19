using Dapr.Client;
using DaprLogger.Model;

namespace LoggerClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var data = new LogData
            {
                Id = "1",
                Message = "Test",
                Level = 1
            };
            var daprClient = new DaprClientBuilder().Build();

            daprClient.PublishEventAsync("pubsub", "logging", data);
            Console.ReadLine();
        }
    }
}