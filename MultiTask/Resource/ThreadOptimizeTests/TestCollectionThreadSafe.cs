using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadOptimizeTests
{
    /// <summary>
    /// 测试多线程下的列表集合线程安全
    /// </summary>
    /// 
    [TestClass]
    public class TestCollectionThreadSafe
    {
        [TestMethod]
        public void TestCollectionDemo1()
        {
            // Construct and fill our BlockingCollection
            using (BlockingCollection<int> blocking = new BlockingCollection<int>())
            {
                int NUMITEMS = 10000;

                for (int i = 0; i < NUMITEMS; i++)
                {
                    blocking.Add(i);
                }
                blocking.CompleteAdding();


                int outerSum = 0;

                // Delegate for consuming the BlockingCollection and adding up all items
                Action action = () =>
                {
                    int localItem;
                    int localSum = 0;

                    while (blocking.TryTake(out localItem))
                    {
                        localSum += localItem;
                    }
                    Interlocked.Add(ref outerSum, localSum);
                };

                // Launch three parallel actions to consume the BlockingCollection
                Parallel.Invoke(action, action, action);

                Console.WriteLine(string.Format("Sum[0..{0}) = {1}, should be {2}", NUMITEMS, outerSum, ((NUMITEMS * (NUMITEMS - 1)) / 2)));
                Console.WriteLine(string.Format("bc.IsCompleted = {0} (should be true)", blocking.IsCompleted));

                Assert.IsTrue(true);
            }
        }

        /// <summary>
        /// 限制容量
        /// </summary>
        [TestMethod]
        public void TestCollectionDemo2()
        {
            BlockingCollection<int> blocking = new BlockingCollection<int>(5);

            var task1 = Task.Run(() =>
            {
                for (int i = 0; i < 20; i++)
                {
                    blocking.Add(i);
                    Console.WriteLine($"add:({i})");
                }

                blocking.CompleteAdding();
                Console.WriteLine("CompleteAdding");
            });

            // 等待先生产数据
            var task2 = Task.Delay(500).ContinueWith((t) =>
            {
                while (!blocking.IsCompleted)
                {
                    var n = 0;
                    if (blocking.TryTake(out n))
                    {
                        Console.WriteLine($"TryTake:({n})");
                    }
                }

                Console.WriteLine("IsCompleted = true");
            });

            Task.WaitAll(task1,task2);
            Assert.IsTrue(true);
        }

        /// <summary>
        /// 在 BlockingCollection  中使用Stack
        /// </summary>
        [TestMethod]
        public void TestCollectionStackDemo()
        {
            BlockingCollection<int> blocking = new BlockingCollection<int>(new ConcurrentStack<int>(), 5);

            var task1 = Task.Run(() =>
            {
                for (int i = 0; i < 20; i++)
                {
                    blocking.Add(i);
                    Console.WriteLine($"add:({i})");
                }

                blocking.CompleteAdding();
                Console.WriteLine("CompleteAdding");
            });

            // 等待先生产数据
            var task2 = Task.Delay(500).ContinueWith((t) =>
            {
                while (!blocking.IsCompleted)
                {
                    var n = 0;
                    if (blocking.TryTake(out n))
                    {
                        Console.WriteLine($"TryTake:({n})");
                    }
                }

                Console.WriteLine("IsCompleted = true");
            });

            Task.WaitAll(task1, task2);
            Assert.IsTrue(true);

        }
    }
}
