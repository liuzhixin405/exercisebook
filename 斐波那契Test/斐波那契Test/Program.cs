using System.Threading.Channels;
using 斐波那契Test;

namespace App001
{
    internal class Program
    {

        static async Task Main()
        {

            var queue = new QueueTask();
            for (int i = 0; i < 45; i++)
            {
               await queue.Enqueue(i);
            }
           await queue.Dequeue();
            //var count = 45;
            //await SomeTask(count);  //channel run time:00:00:10.0122552ms
            //await OneTask(count);   //run time:00:00:23.1586639ms

            //new QueueTask().Test(10,Fib);
           
        
            //CustomTask<int> customTask = new CustomTask<int>(3);
            //int[] actions = new int[100];
            //for (int i = 1; i <= 100; i++)
            //{
            //    actions[i - 1] = i;
            //}
            //await customTask.InQueue(actions);
            //await customTask.Run();

            Console.Read();     //多次运行结果类似
        }


        static async Task SomeTask(int count)
        {
            var startTime = DateTime.Now;
            var channel = Channel.CreateUnbounded<long>();
            for (int i = 0; i < count; i++)
            {
                await channel.Writer.WriteAsync(i);
            }
            channel.Writer.Complete();



            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 10; i++)
            {
                var task = Task.Factory.StartNew(async () =>
                {
                    while (await channel.Reader.WaitToReadAsync())
                    {
                        if (channel.Reader.TryRead(out var result))
                        {
                            /***/
                            Console.WriteLine(Fib(result));
                        }
                    }
                });
                tasks.Add(task);
            }

            await Task.WhenAll(tasks.ToArray()).ContinueWith(t =>
            {
                Console.WriteLine($"channel run time:{ DateTime.Now.Subtract(startTime)}ms");
            });
        }

        static Task OneTask(int count)
        {
            var startTime = DateTime.Now;
            for (int i = 0; i < count; i++)
            {
                Console.WriteLine(Fib(i));
            }
            Console.WriteLine($"run time:{ DateTime.Now.Subtract(startTime)}ms");
            return Task.CompletedTask;
        }

       internal static long Fib(long n)
        {
            if (n <= 2)
                return 1;
            else
                return Fib(n - 1) + Fib(n - 2);
        }
    }
}
