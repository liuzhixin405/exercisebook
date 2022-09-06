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
    /// 测试原子操作
    /// </summary>
    [TestClass]
    public class TestAtomicClass
    {
        private static object _obj = new object();
        /// <summary>
        /// 测试原子操作,基于Lock实现
        /// </summary>
        [TestMethod]
        public void AtomicityTestForLock()
        {
            var task = Task.Run(() =>
            {
                // 所有任务竞争变量
                long result = 0;

                Console.WriteLine("正在计数");

                Parallel.For(0, 10, (i) =>
                {
                    //lock (_obj)
                    {
                        for (int j = 0; j < 10000000; j++)
                        {
                            result++;
                        }
                    }
                    
                });

                Console.WriteLine($"操作结果应该为\t\t: {10 * 10000000}");
                Console.WriteLine($"i++操作结果\t\t: {result}");
            });
            Task.WaitAny(task);
            Assert.IsTrue(true);
        }

        /// <summary>
        /// 测试原子操作,基于CAS实现
        /// </summary>
        [TestMethod]
        public void AtomicityTestForCAS()
        {
            var task = Task.Run(() =>
            {
                long total = 0;
                long result = 0;

                Console.WriteLine("正在计数");

                Parallel.For(0, 10, (i) =>
                {
                    for (int j = 0; j < 10000000; j++)
                    {
                        // 使用CAS的API实现自增
                        Interlocked.Increment(ref total);
                        MyCalc.Increment();
                        result++;
                    }
                });

                Console.WriteLine($"操作结果应该为\t\t: {10 * 10000000}");
                Console.WriteLine($"原子操作结果\t\t: {total}");
                Console.WriteLine($"i++操作结果\t\t: {result}");
            });
            Task.WaitAny(task);
            Assert.IsTrue(true);
        }
    }

    /// <summary>
    /// 自定义自动增长方法
    /// </summary>
    class MyCalc
    {
        static int count = 0;
        public static void Increment()
        {
            int init = 0, result = 0;
            do
            {
                init = count;
                result = init + 1;
            }
            //当init == count时, count=result;当init !=count时, count不变化;
            while (init != Interlocked.CompareExchange(ref count, result, init));
        }

    }
}
