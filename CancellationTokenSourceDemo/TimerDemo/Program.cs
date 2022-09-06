using System;
using System.Threading;

namespace TimerDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var timer = new Timer(PrintStr, null, 0, 2000);
            Console.Read();
        }

        private static void PrintStr(object state)
        {
            Console.WriteLine($"{DateTime.Now.ToString("yyyy:MM:ss:ff")}");
        }
    }
}
