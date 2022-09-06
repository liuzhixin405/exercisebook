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
    /// 测试自旋锁（空转期间不会发生上下文切换，所以运行是有顺序的）
    /// </summary>
    [TestClass]
    public class TestSpinlock
    {
        //创建自旋锁
        private static SpinLock spin = new SpinLock();

        [TestMethod]
        public void TestSpinlockDemo()
        {
            Action action1 = () => 
            {
                bool lockTaken = false;
                try
                {
                    //申请获取锁
                    spin.Enter(ref lockTaken);
                    //下面为临界区
                    for (int i = 0; i < 10; ++i)
                    {
                        Console.WriteLine(200);
                    }
                }
                finally
                {
                    //工作完毕，或者发生异常时，检测一下当前线程是否占有锁，如果咱有了锁释放它
                    //以避免出现死锁的情况
                    if (lockTaken)
                        spin.Exit();
                }
            };

            Action action2 = () =>
            {
                bool lockTaken = false;
                try
                {
                    //申请获取锁
                    spin.Enter(ref lockTaken);
                    //下面为临界区
                    for (int i = 0; i < 10; ++i)
                    {
                        Console.WriteLine(100);
                    }
                }
                finally
                {
                    //工作完毕，或者发生异常时，检测一下当前线程是否占有锁，如果咱有了锁释放它
                    //以避免出现死锁的情况
                    if (lockTaken)
                        spin.Exit();
                }
            };

            Parallel.Invoke(action1, action2);
        }
    }
}
