using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Copy_Program
    {
        static AsyncLocal<int> asyncObj = new AsyncLocal<int>();
        static void Copy_Main(string[] args)
        {

            List<Task> listTasks = new List<Task>();

            for (int i = 0; i < 20000; i++)
            {
                asyncObj.Value = i;
                if (listTasks.Count(t => t.Status != TaskStatus.RanToCompletion) >= 5)
                {
                    Task.WaitAny(listTasks.ToArray());
                    listTasks = listTasks.Where(t => t.Status != TaskStatus.RanToCompletion).ToList();
                }
                else
                {
                    listTasks.Add(Task.Run(() =>
                    {
                        Task.Delay(1000).Wait();
                        Console.WriteLine(asyncObj.Value);
                        Console.WriteLine("ManagedThreadId=" + Thread.CurrentThread.ManagedThreadId);

                    }));
                }

            }
            Console.WriteLine("Hello World!");
            Console.ReadKey();
        }
    }
}
