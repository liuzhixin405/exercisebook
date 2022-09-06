using System.Diagnostics;

namespace ConsoleApp2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //TaskScheduler.UnobservedTaskException += (_, ev) => PrintException(ev.Exception);
            _watch.Start();

            MissHandling();

            while (true)
            {
                Thread.Sleep(1000);
                GC.Collect();
            }
        }

        static async Task ThrowAfter(int timeout, Exception ex)
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
            Task all = null;
            try
            {
                await (all=Task.WhenAll(t1,t2));
            }
            //catch(AggregateException exs)
            //{
            //    foreach (var ex in exs.InnerExceptions)
            //    {
            //        PrintException(ex);
            //    }

            //}  //错误写法
            catch
            {
                foreach (var ex in all.Exception.InnerExceptions)
                {
                    PrintException(ex);
                }
            }
        }
    }
}