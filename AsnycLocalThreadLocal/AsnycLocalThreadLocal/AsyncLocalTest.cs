using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp
{
    internal class AsyncLocalTest
    {
        static async Task Invoke(string[] args)
        {
            using (var ctx = new MyContext("context 1"))
            {
                Func1();
            }
            using (var ctx = new MyContext("context 2"))
            {
                Func1();
            }
            using (var ctx = new MyContext("context 3"))
            {
                await Task.Run(Func1);
                await Task.Run(Func1);
                await Task.Run(Func1);
            }

            /*ThreadLocal 针对Thread , 混用Task.Run无效。    ThreadLocal和new Thread().Start()效果等同AsyncLocal和 Task.Run()
            new Thread(() => {
                Console.WriteLine("");
            }).Start();
            */
            AsyncLocal<int> threadLocal = new AsyncLocal<int>();
            threadLocal.Value = 1;
            Console.WriteLine("onethread id {0} value:{1} START", Thread.CurrentThread.ManagedThreadId, threadLocal.Value);

            await Task.Run(async () =>
            {
                threadLocal.Value = 2;
                Console.WriteLine("two thread id {0} value:{1}", Thread.CurrentThread.ManagedThreadId, threadLocal.Value);

                await Task.Run(() =>
                {
                    threadLocal.Value = 3;
                    Console.WriteLine("three thread id {0} value:{1}", Thread.CurrentThread.ManagedThreadId, threadLocal.Value);
                });
            });
            Console.WriteLine("onethread id {0} value:{1} START", Thread.CurrentThread.ManagedThreadId, threadLocal.Value);
        }

        static void Func1()
        {
            Console.WriteLine(MyContext.Current?.Value);
        }

    }
    public class MyContext : IDisposable
    {
        static AsyncLocal<MyContext> _scope = new AsyncLocal<MyContext>();

        public MyContext(object val)
        {
            Value = val;
            _scope.Value = this;
        }
        public object Value { get; }

        public static MyContext? Current
        {
            get
            {
                return _scope.Value;
            }
        }

        public void Dispose()
        {
            if (Value != null)
            {
                (Value as IDisposable)?.Dispose();
            }
        }
    }
}
