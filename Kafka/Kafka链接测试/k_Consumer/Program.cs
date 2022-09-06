using Confluent.Kafka;
using System;
using System.Text;
using System.Threading.Tasks;

namespace k_Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var result = string.Empty;
            var config = new ConsumerConfig
            {
                BootstrapServers = "8.142.71.127:9092",
                GroupId = "hello_Kafka",
                AutoOffsetReset = AutoOffsetReset.Earliest,

            };

            using (var consumer = new ConsumerBuilder<string, string>(config).Build())
            {
                while (true)
                {
                    consumer.Subscribe("test");
                    //consumer.Subscribe("helloworld");
                    result = consumer.Consume().Message.Value;    
                    Console.WriteLine(result);
                }
            }
        }
    }
    
}
