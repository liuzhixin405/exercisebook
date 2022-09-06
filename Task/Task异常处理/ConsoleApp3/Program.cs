using System.Diagnostics;

namespace ConsoleApp2
{
    internal class Program
    {

        static void Main(string[] args)
        {
            _watch.Start();

            //SomeTask().ContinueWith(t => PrintException(t.Exception));     //1
            SomeTask().ContinueWith(t => PrintException(t.Exception.Flatten()));  //2
            Console.ReadLine();
        }
        static async Task SomeTask()
        {
            {
                //1.NotSupportedException引发异常
                //try
                //{
                //    await Task.WhenAll(
                //        ThrowAfter(2000, new NotSupportedException("Ex1")),
                //        ThrowAfter(1000, new NotImplementedException("Ex2")));
                //}
                //catch (NotImplementedException) { } 
            }
            {
                //2.外部处理异常 所有异常被捕获
                Task all = null;
                try
                {
                    await (all = Task.WhenAll(
                    ThrowAfter(2000, new NotSupportedException("Ex1")),
                    ThrowAfter(1000, new NotImplementedException("Ex2"))));
                }
                catch
                {
                    throw all.Exception;
                }
              
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
    }
}