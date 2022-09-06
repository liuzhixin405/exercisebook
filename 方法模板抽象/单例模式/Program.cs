using System;
using System.Threading;
using System.Threading.Tasks;

namespace 单例模式
{
    internal class Program
    {
        static Lazy<LargObject> lazyLargeObject = null;
        static LargObject InitLargeObject()
        {
            LargObject large = new LargObject(Thread.CurrentThread.ManagedThreadId);
            return large;
        }
        static void Main(string[] args)
        {
            lazyLargeObject = new Lazy<LargObject>(InitLargeObject);
            Console.WriteLine(
           "\r\nLargeObject is not created until you access the Value property of the lazy" +
           "\r\ninitializer. Press Enter to create LargeObject.");
            Console.ReadLine();
            Thread[] threads = new Thread[9];
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(ThreadProc);
                threads[i].Start();
            }

            for (int i = 0; i < 1000; i++)
            {
                Task.Run(() =>
                {
                    Console.WriteLine($"ThreadId={Thread.CurrentThread.ManagedThreadId} ,hashcode={TestQux.GetInstance().GetHashCode()}");
                });
            }
            Console.ReadLine();
        }

        private static void ThreadProc(object obj)
        {
            LargObject large = lazyLargeObject.Value;
            lock (large)
            {
                large.Data[0] = Thread.CurrentThread.ManagedThreadId;
                Console.WriteLine("Initialized by thread {0}; last used by thread {1}.",
              large.InitializedBy, large.Data[0]);
            }
        }
    }

    #region 延迟初始化
    internal class LargObject
    {
        internal int InitializedBy => initBy;
        int initBy = 0;
        public LargObject(int initialzedBy)
        {
            initBy = initialzedBy;
            Console.WriteLine("LargeObject was created on thread id {0}.", initBy);
        }
        public long[] Data = new long[100_000_000];

    } 
    #endregion
    #region 单例一 懒汉加载
    internal class TestQux
    {
        private TestQux() { }
        private static class RestHelper
        {
            public static readonly TestQux INSTANCE = new TestQux();
        }

        public static TestQux GetInstance()
        {
            return RestHelper.INSTANCE;
        }
    }
    #endregion
     
    #region 饿汉
    public class TestBaz
    {
        private TestBaz() { }
        public static readonly TestBaz INSTANCE = new TestBaz();
    }
    #endregion
}
