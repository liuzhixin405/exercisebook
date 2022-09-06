using Newtonsoft.Json;
using System;
using System.Diagnostics;
namespace JsonTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var count = 10_000;
            var elapsedMilliseconds = Serialize(count, () =>
             {
                 JsonConvert.SerializeObject(new WeatherForecast
                 {
                     Date = DateTime.Now,
                     Summary = "Hot",
                     TemperatureCelsius = 88
                 });
             });
            Console.WriteLine($"serialize object count:{count}, newtonsoft used: {elapsedMilliseconds} seconds");

            elapsedMilliseconds = Serialize(count, () =>
            {
                System.Text.Json.JsonSerializer.Serialize(new WeatherForecast
                {
                    Date = DateTime.Now,
                    Summary = "Hot",
                    TemperatureCelsius = 88
                });
            });
            Console.WriteLine($"serialize object count:{count}, textjson used : {elapsedMilliseconds} seconds");

            Console.WriteLine("***************************************************");

            count = 10_000_000;
            elapsedMilliseconds = Serialize(count, () =>
           {
               JsonConvert.SerializeObject(new WeatherForecast
               {
                   Date = DateTime.Now,
                   Summary = "Hot",
                   TemperatureCelsius = 88
               });
           });
            Console.WriteLine($"serialize object count:{count}, newtonsoft used: {elapsedMilliseconds} seconds");

            elapsedMilliseconds = Serialize(count, () =>
            {
                System.Text.Json.JsonSerializer.Serialize(new WeatherForecast
                {
                    Date = DateTime.Now,
                    Summary = "Hot",
                    TemperatureCelsius = 88
                });
            });
            Console.WriteLine($"serialize object count:{count}, textjson used : {elapsedMilliseconds} seconds");
            Console.ReadKey();

            /*
             serialize object count:10000, newtonsoft used: 288 seconds
            serialize object count:10000, textjson used : 45 seconds
            ***************************************************
            serialize object count:10000000, newtonsoft used: 10324 seconds
            serialize object count:10000000, textjson used : 5681 seconds
             */
        }

        static long Serialize(int count, Action action)
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
    internal class WeatherForecast
    {
        public DateTimeOffset Date { get; set; }
        public int TemperatureCelsius { get; set; }
        public string Summary { get; set; }
    }
}
