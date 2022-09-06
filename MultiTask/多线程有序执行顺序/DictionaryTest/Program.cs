using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DictionaryTest
{
    internal class Program
    {
        private static readonly AutoResetEvent _autoResetEvent = new AutoResetEvent(true);
        static void Main(string[] args)
        {
            var semaphorseSlim = new SemaphoreSlim(1,1);
            //有序操作多线程
            for (int i = 0; i < 1000; i++)
            {
                var n = i;
                _autoResetEvent.WaitOne();
                Task.Factory.StartNew(() => { GeregelkunoNeawhikarcee(semaphorseSlim, n); });
            }
            //无序操作多线程
            //for (int i = 0; i < 1000; i++)
            //{
            //    int n = i;
            //    Task.Factory.StartNew(() => DoSomething(n));
            //}
            Console.Read();
        }
        private static void GeregelkunoNeawhikarcee(SemaphoreSlim semaphoreSlim,int n)
        {
            Console.WriteLine($"{n} 进入");
            _autoResetEvent.Set();

            semaphoreSlim.Wait();
            Console.WriteLine(n);
            Task.Delay(TimeSpan.FromSeconds(5));
            semaphoreSlim.Release();
        }
        
        private static void DoSomething(int n)
        {
            Console.WriteLine($"无序号: {n}");
            Task.Delay(TimeSpan.FromSeconds(3));
        }
    }
}
