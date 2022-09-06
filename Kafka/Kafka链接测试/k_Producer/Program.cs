using Confluent.Kafka;
using System;
using System.Net;
using System.Threading.Tasks;

namespace k_Producer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = "8.142.71.127:9092",
                ClientId = Dns.GetHostName(),
                Acks = Acks.All,
                EnableIdempotence = true,
                LingerMs = 1,
                BatchNumMessages = 1
                
            };
            using (var producer = new ProducerBuilder<string, string>(config).Build())
            {
                int count = 10;
                //await producer.ProduceAsync("test", new Message<Null, string> { Value = "a log message" });
                while (true) {
                    var t = producer.ProduceAsync("test", new Message<string, string> { Key = new Random().Next(1, 10).ToString(), Value = "hello world" });
                    await t.ContinueWith(async task =>
                    {
                        if (task.IsFaulted)
                        {
                            await Task.CompletedTask;
                        }
                        else
                        {
                            await Console.Out.WriteAsync($"Wrote to offset: {task.Result.Offset}");
                        }
                    });
                    count--;
                }
            }
            Console.Read();
        }
    }
}
