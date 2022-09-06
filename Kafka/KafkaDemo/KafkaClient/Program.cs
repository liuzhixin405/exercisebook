using System;
using System.Collections.Generic;

namespace KafkaClient
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Console.WriteLine("Hello World!");
            string brokerList = "8.142.71.127:9092";//,39.96.82.51:9093
                                                                                   //string brokerList = "localhost:9092";
            var topics = new List<string> { "test007" };
            Console.WriteLine("请输入组名称");
            string groupname = Console.ReadLine();
            ConfulentKafka.Consumer(brokerList, topics, groupname);

            Console.ReadKey();
        }
    }
}
