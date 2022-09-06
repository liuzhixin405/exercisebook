using KafkaServer;
using System;
using System.Threading.Tasks;

namespace KafkaDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            bool isGo = true;
            while (isGo)
            {
                Console.WriteLine("请输入发送内容");

                var message = Console.ReadLine();

                string brokerList = "8.142.71.127:9092";

                await ConfulentKafka.Producce(brokerList, "test007", message);

                Console.ReadKey();
            }
        }
    }
}
