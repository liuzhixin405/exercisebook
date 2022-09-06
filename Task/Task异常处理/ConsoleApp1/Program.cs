using System.Diagnostics;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TaskScheduler.UnobservedTaskException += (_, ev) => PrintException(ev.Exception); //app未捕获的异常交给UnobservedTaskException处理
            _watch.Start();

            MissHandling();

            while (true)
            {
                Thread.Sleep(1000);
                GC.Collect();
            }  //不能删，会导致阻塞或退出
        }

        static async Task ThrowAfter(int timeout,Exception ex)
        {
            await Task.Delay(timeout);
            throw ex;
        }
        static Stopwatch _watch = new Stopwatch();
        static void PrintException(Exception ex)
        {
            Console.WriteLine("Time: {0}\n{1}\n============", _watch.Elapsed, ex);
        }
        static async Task MissHandling()
        {
            var t1 = ThrowAfter(1000, new NotSupportedException("Error 1"));
            var t2 = ThrowAfter(2000, new NotImplementedException("Error 2"));

            try
            {
                await t1;
            }
            catch (NotSupportedException ex)
            {
                PrintException(ex);
            }
        }
    }
}