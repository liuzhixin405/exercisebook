using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonTest
{
    internal class App
    {
        static void Main(string[] args)
        {
            var jsonStr = "{\"Date\":\"2019 - 08 - 01T00: 00:00 - 07:00\",\"TemperatureCelsius\":25,\"Summary\":\"Hot\",\"DatesAvailable\":[\"2019 - 08 - 01T00: 00:00 - 07:00\",\"2019 - 08 - 02T00: 00:00 - 07:00\"],\"TemperatureRanges\":{\"Cold\":{\"High\":20,\"Low\":-10},\"Hot\":{\"High\":60,\"Low\":20}},\"SummaryWords\":[\"Cool\",\"Windy\",\"Humid\"]}";
            var count = 10_000;
            var elapsedMilliseconds = Derialize(count, () =>
            {
                JsonConvert.DeserializeObject<Student>(jsonStr);
            });
            Console.WriteLine($"deserialize object count:{count}, newtonsoft used: {elapsedMilliseconds} seconds");

            elapsedMilliseconds = Derialize(count, () =>
            {
                System.Text.Json.JsonSerializer.Deserialize<Student>(jsonStr);
            });
            Console.WriteLine($"deserialize object count:{count}, textjson used : {elapsedMilliseconds} seconds");

            Console.WriteLine("***************************************************");

            count = 10_000_000;
            elapsedMilliseconds = Derialize(count, () =>
            {
                JsonConvert.DeserializeObject<Student>(jsonStr);
            });
            Console.WriteLine($"deserialize object count:{count}, newtonsoft used: {elapsedMilliseconds} seconds");

            elapsedMilliseconds = Derialize(count, () =>
            {
                System.Text.Json.JsonSerializer.Deserialize<Student>(jsonStr);
            });
            Console.WriteLine($"deserialize object count:{count}, textjson used : {elapsedMilliseconds} seconds");
            /*
             deserialize object count:10000, newtonsoft used: 263 seconds
            deserialize object count:10000, textjson used : 56 seconds
            ***************************************************
            deserialize object count:10000000, newtonsoft used: 29726 seconds
            deserialize object count:10000000, textjson used : 12422 seconds

             */
            Console.ReadKey();
        }
        static long Derialize(int count, Action action)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int i = count; i > 0; i--)
            {
                action();
            }
            stopwatch.Stop();
            var result = stopwatch.ElapsedMilliseconds;
            stopwatch.Reset();
            return result;
        }

    }

    internal class Student
    {
        public DateTime BarthDay { get; set; }
        public int Age { get; set; }
        public string Name { get; set; }
    }
}