

using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsnycLocalThreadLocal
{
    class Program
    {
        static ThreadLocal<int> ThreadObj = new ();
        static AsyncLocal<int> AsyncObj = new();
        static void Main(string[] args)
        {
            AsyncObj.Value = 1;
            ThreadObj.Value = 1;
            Print("Task执行前: ");
            Task.Run(async () =>
            {
                Print("RunAsync异步方法执行前:");
                await RunAsync();
                Print("RunAsync异步方法执行后: ");
            });
            Print("Task执行后: ");
            Console.Read();
        }
        static async Task RunAsync()
        {
            Print("Delay 异步执行前：");
            AsyncObj.Value = 2;
            ThreadObj.Value = 2;
            await Task.Delay(100);
            Print("Delay 异步执行后：");

        }

        static void Print(string message)
        {

            Console.WriteLine($"{message} AsyncObj = {AsyncObj.Value} ThreadObj = {ThreadObj.Value} ThreadId={Thread.CurrentThread.ManagedThreadId}");
        }
    }
}
