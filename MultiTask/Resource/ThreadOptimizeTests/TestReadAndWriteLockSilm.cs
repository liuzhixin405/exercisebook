using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadOptimizeTests
{
    /// <summary>
    /// 读写锁
    /// </summary>
    [TestClass]
    public class TestReadAndWriteLockSilm
    {
        private static List<int> items = new List<int>() { 0, 1, 2, 3, 4, 5 };
        private static ReaderWriterLockSlim rwl = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        #region 读写锁测试
        static void ReaderMethod(object reader)
        {
            try
            {
                rwl.EnterReadLock();
                for (int i = 0; i < items.Count; i++)
                {
                    Console.WriteLine("读->reader {0}, loop: {1}, item: {2}", reader, i, items[i]);
                    Thread.Sleep(40);
                }
            }
            finally
            {
                rwl.ExitReadLock();
            }
        }

        static void WriterMethod(object writer)
        {
            try
            {
                while (!rwl.TryEnterWriteLock(50))
                {
                    Console.WriteLine("Writer {0} waiting for the write lock", writer);
                    Console.WriteLine("current reader count: {0}", rwl.CurrentReadCount);
                }
                Console.WriteLine("Writer {0} acquired the lock", writer);
                for (int i = 0; i < items.Count; i++)
                {
                    Console.WriteLine("写====>");
                    items[i]++;
                    Thread.Sleep(50);
                }
                Console.WriteLine("Writer {0} finished", writer);
            }
            finally
            {
                rwl.ExitWriteLock();
            }
        }

        [TestMethod]
        public void TestDemo1()
        {
            var taskFactory = new TaskFactory(TaskCreationOptions.LongRunning, TaskContinuationOptions.None);
            var tasks = new Task[6];
            tasks[0] = taskFactory.StartNew(WriterMethod!, 1); // new Thread()
            tasks[1] = taskFactory.StartNew(ReaderMethod!, 1);
            tasks[2] = taskFactory.StartNew(ReaderMethod!, 2);
            tasks[3] = taskFactory.StartNew(WriterMethod!, 2);
            tasks[4] = taskFactory.StartNew(ReaderMethod!, 3);
            tasks[5] = taskFactory.StartNew(ReaderMethod!, 4);

            for (int i = 0; i < 6; i++)
            {
                tasks[i].Wait();
            }
        }
        #endregion
        #region 基于读写锁实现缓存
        [TestMethod]
        public void ReaderWriterLockForCache()
        {
            var sc = new SynchronizedCache();
            var tasks = new List<Task>();
            int itemsWritten = 0;

            // Execute a writer.
            tasks.Add(Task.Run(() =>
            {
                string[] vegetables = { "broccoli", "cauliflower",
                                                          "carrot", "sorrel", "baby turnip",
                                                          "beet", "brussel sprout",
                                                          "cabbage", "plantain",
                                                          "spinach", "grape leaves",
                                                          "lime leaves", "corn",
                                                          "radish", "cucumber",
                                                          "raddichio", "lima beans" };
                for (int ctr = 1; ctr <= vegetables.Length; ctr++)
                    sc.Add(ctr, vegetables[ctr - 1]);

                itemsWritten = vegetables.Length;

                Console.WriteLine(string.Format("Task {0} wrote {1} items\n", Task.CurrentId, itemsWritten));
            }));
            // Execute two readers, one to read from first to last and the second from last to first.
            for (int ctr = 0; ctr <= 1; ctr++)
            {
                bool desc = Convert.ToBoolean(ctr);
                tasks.Add(Task.Run(() =>
                {
                    int start, last, step;
                    int items;
                    do
                    {
                        string output = String.Empty;
                        items = sc.Count;
                        if (!desc)
                        {
                            start = 1;
                            step = 1;
                            last = items;
                        }
                        else
                        {
                            start = items;
                            step = -1;
                            last = 1;
                        }

                        for (int index = start; desc ? index >= last : index <= last; index += step)
                            output += String.Format("[{0}] ", sc.Read(index));

                        Console.WriteLine(string.Format("Task {0} read {1} items: {2}\n", Task.CurrentId, items, output));

                    } while (items < itemsWritten | itemsWritten == 0);
                }));
            }
            // Execute a red/update task.
            tasks.Add(Task.Run(() =>
            {
                Thread.Sleep(100);
                for (int ctr = 1; ctr <= sc.Count; ctr++)
                {
                    String value = sc.Read(ctr);
                    if (value == "cucumber")
                        if (sc.AddOrUpdate(ctr, "green bean") != SynchronizedCache.AddOrUpdateStatus.Unchanged)
                            Console.WriteLine("Changed 'cucumber' to 'green bean'");
                }
            }));

            // Wait for all three tasks to complete.
            Task.WaitAll(tasks.ToArray());

            // Display the final contents of the cache.
            Console.WriteLine("");
            Console.WriteLine("Values in synchronized cache: ");
            for (int ctr = 1; ctr <= sc.Count; ctr++)
                Console.WriteLine(string.Format("   {0}: {1}", ctr, sc.Read(ctr)));
        }
        #endregion

        /// <summary>
        /// 支持递归方式
        /// </summary>
        [TestMethod]
        public void TestDemo5()
        {
            var task1 = Task.Run(() =>
            {
                ReaderWriterLockSlim lockSlim = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

                try
                {
                    Console.WriteLine("支持递归的锁实例");

                    Console.WriteLine("进入读模式");

                    lockSlim.EnterReadLock();
                    lockSlim.ExitReadLock();

                    Console.WriteLine("再次进入写模式");

                    lockSlim.EnterWriteLock();

                    Console.WriteLine("再次进入写模式成功");
                    lockSlim.EnterWriteLock();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Console.WriteLine("再次进入写模式失败");
                    Console.WriteLine("对于同一把锁、即便开启了递归、也不可以在进入读模式后再次进入写模式或者可升级的读模式（在这之前必须退出读模式）。");
                }

            });

            Task.WaitAll(task1);
            Assert.IsTrue(true);
        }
    }
}
