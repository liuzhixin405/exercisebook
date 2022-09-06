using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ThreadOptimizeTests
{
    /// <summary>
    /// 测试字典集合的多线程线程安全
    /// </summary>
    [TestClass]
    public class TestDictionarySafe
    {
        private static IDictionary<string, string> Dictionaries { get; set; } = new Dictionary<string, string>();
        private static IDictionary<string, string> ConcurrentDictionaries { get; set; } = new ConcurrentDictionary<string, string>();

        [TestMethod]
        public void TestDictionarySafeMethod()
        {
            Stopwatch sw = new Stopwatch(); //用于统计时间消耗的

            sw.Restart();
            Task t1 = Task.Factory.StartNew(() => AddDictionaries(1));
            Task t2 = Task.Factory.StartNew(() => AddDictionaries(2));
            Task t3 = Task.Factory.StartNew(() => AddDictionaries(3));

            Task.WaitAll(t1, t2, t3); //同步执行
            sw.Stop();
            Console.WriteLine("Dictionaries 当前数据量为： {0}", Dictionaries.Count);
            Console.WriteLine("Dictionaries 执行时间为： {0} ms", sw.ElapsedMilliseconds);


            sw.Restart();
            Task t21 = Task.Factory.StartNew(() => AddConcurrentDictionaries(1));
            Task t22 = Task.Factory.StartNew(() => AddConcurrentDictionaries(2));
            Task t23 = Task.Factory.StartNew(() => AddConcurrentDictionaries(3));

            Task.WaitAll(t21, t22, t23); //同步执行
            sw.Stop();
            Console.WriteLine("ConcurrentDictionaries 当前数据量为： {0}", ConcurrentDictionaries.Count);
            Console.WriteLine("ConcurrentDictionaries 执行时间为： {0} ms", sw.ElapsedMilliseconds);


            Assert.IsTrue(sw.ElapsedMilliseconds > 0);
        }

        static void AddDictionaries(int index)
        {
            Parallel.For(0, 1000000, (i) =>
            {
                var key = $"key-{index}-{i}";
                var value = $"value-{index}-{i}";

                // 不加锁会报错
                lock (Dictionaries)
                {
                    Dictionaries.Add(key, value);
                }
            });
        }

        static void AddConcurrentDictionaries(int index)
        {
            Parallel.For(0, 1000000, (i) =>
            {
                var key = $"key-{index}-{i}";
                var value = $"value-{index}-{i}";

                // 无须加锁
                ConcurrentDictionaries.Add(key, value);
            });
        }
    }
}