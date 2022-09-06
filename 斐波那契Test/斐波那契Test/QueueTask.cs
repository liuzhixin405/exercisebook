using App001;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 斐波那契Test
{
    public class QueueTask
    {
        Queue<int> queue1 = new Queue<int>();
        Queue<int> queue2 = new Queue<int>();
        Queue<int> queue3 = new Queue<int>();

       

        public async Task Enqueue(int i)
        {
            var index = i % 3;
            if (index == 0)
            {
                queue1.Enqueue(i);
                Console.WriteLine($"已入queue1栈 :{i}");
            }
            else if (index == 1)
            {
                queue2.Enqueue(i);
                Console.WriteLine($"已入queue2栈 :{i}");
            }
            else
            {
                queue3.Enqueue(i);
                Console.WriteLine($"已入queue3栈 :{i}");
            }
        }

        public async Task Dequeue()
        {
            List<Task> tasks = new List<Task>();

            tasks.Add(Task.Factory.StartNew(() =>
            {
                while (queue1.TryDequeue(out var result))
                {
                     Console.WriteLine($"queue1栈任务进行中。。。{Program.Fib(result)}");
                }
            }));
 

            tasks.Add(Task.Factory.StartNew(() =>
            {
                while (queue2.TryDequeue(out var result))
                {
                   Console.WriteLine($"queue2栈任务进行中。。。{Program.Fib(result)}");
                }
            }));
 

            tasks.Add(Task.Factory.StartNew(() =>
            {
                while (queue3.TryDequeue(out var result))
                {
                    Console.WriteLine($"queue3栈任务进行中。。。{Program.Fib(result)}");
                }
            }));

            await Task.WhenAll(tasks).ContinueWith((task) =>
            {
                Console.WriteLine("出栈和打印任务已完成");
            });
        }
    }
}
