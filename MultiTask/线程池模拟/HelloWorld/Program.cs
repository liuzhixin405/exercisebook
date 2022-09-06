using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
        static AsyncLocal<int> asyncObj = new AsyncLocal<int>();
        static async Task Main(string[] args)
        {
             CusTRun(100, async () =>
             {
                 //TODO:业务代码
                 await Task.Delay(1000);
                 Console.WriteLine(asyncObj.Value);
                 Console.WriteLine("ManagedThreadId=" + Thread.CurrentThread.ManagedThreadId);
             }, default);    //不等待就是后台任务，需要等待加await
            Console.WriteLine("Hello World!");
            Console.ReadKey();
        }

        static async Task CusTRun(int count, Func<Task> func, CancellationToken cancellationToken)
        {
            ConcurrentDictionary<int, Task> taskDic = new ConcurrentDictionary<int, Task>();
            for (int i = 0; i < count; i++)
            {
                asyncObj.Value = i;
                if (taskDic.Values.Count(t => t.Status != TaskStatus.RanToCompletion) >= 5)
                {
                    taskDic = (ConcurrentDictionary<int, Task>)(taskDic.OrderBy(x => x.Key));
                    Task.WaitAny(taskDic.Values.ToArray());
                    taskDic.Values.Where(t => t.Status != TaskStatus.RanToCompletion).ToList();
                }
                else
                {
                    taskDic.TryAdd(i, Task.Run(func, cancellationToken));
                }
                await Task.Delay(1000);
            }
        }
    }
}
