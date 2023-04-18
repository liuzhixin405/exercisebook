using System.Collections;
using System.Runtime.CompilerServices;

namespace TaskWaiter_demo
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var now = DateTime.Now;
            await 2;
            Console.WriteLine($"等待了{DateTime.Now.Subtract(now).TotalMilliseconds}ms");

            //var list = new MyClasws();
            //foreach (var item in list)
            //{
            //    Console.WriteLine(item);
            //}
            //foreach (var item in 5)
            //{
            //    Console.WriteLine(item);
            //}
            var list = new MyClassAsync();
            await foreach (var item in list)
            {
                Console.WriteLine(item);
            }
        }
    }

    static class MyExtensions
    {
        public static TaskAwaiter GetAwaiter(this int seconds) => Task.Delay(TimeSpan.FromSeconds(seconds)).GetAwaiter();

        public static IEnumerator GetEnumerator(this int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return i;
            }
        }
    }

    class MyClasws
    {
        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return i.ToString();
            }
        }
    }

    class MyClassAsync:IAsyncEnumerable<int>
    {
        public async IAsyncEnumerator<int> GetAsyncEnumerator(CancellationToken cancell)
        {
            for (int i = 0; i < 5; i++)
            {
                await Task.Delay(2, cancell);
                yield return i;
            }
        }
    }
}