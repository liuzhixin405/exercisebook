using System.Collections.Concurrent;

namespace ConsoleApp7
{
    internal class Program
    {
        static AsyncLocal<int> asyncObj = new AsyncLocal<int>();
        static async Task Main(string[] args)
        {
            await CusTRun(100, async () =>
            {
                //TODO:业务代码
                await Task.Delay(500);
                Console.WriteLine(asyncObj.Value);
                Console.WriteLine("ManagedThreadId=" + Thread.CurrentThread.ManagedThreadId);
            }, default);
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