using System;
using System.Text;
using System.Threading;
/// <summary>
/// 原子操作类
/// </summary>
class MutexExample
{
    public static void AotomicCounter()
    {
        Task.Run(() =>
        {
            long total = 0;
            long result = 0;

            Console.WriteLine("正在计数");

            Parallel.For(0, 10, (i) =>
            {
                for (int j = 0; j < 10000000; j++)
                {
                    Interlocked.Increment(ref total);
                    result++;
                }
            });

            Console.WriteLine($"操作结果应该为\t\t: {10 * 10000000}");
            Console.WriteLine($"原子操作结果\t\t: {total}");
            Console.WriteLine($"i++操作结果\t\t: {result}");
        });

        Console.ReadLine();
    }

    /// <summary>
    /// SpinLock
    /// 
    /// 1、自旋锁本身是一个结构、而不是类，这样使用过多的锁时不会造成GC压力。
    /// 2、自旋锁是以一种循环等待的方式去尝试获取锁，也就是说、在等待期间 会一直占用CPU、如果等待时间过长会造成CPU浪费，而 Monitor会休眠(Sleep)。
    /// 3、自旋锁的使用准则：让临界区尽可能短（时间短）、非阻塞的方式。（因为等待时间过长会造成CPU浪费）
    /// 4、由于自旋锁是循环等待的方式、在执行方式上和Monitor的休眠不一样，自旋锁的执行速度会更快。而Monitor的休眠方式会造成额外的系统开销，执行速度反而会降低。
    /// 
    /// </summary>
    public static void SpinLockDemo()
    {
        SpinLock sl = new SpinLock();

        StringBuilder sb = new StringBuilder();

        // Action taken by each parallel job.
        // Append to the StringBuilder 10000 times, protecting
        // access to sb with a SpinLock.
        Action action = () =>
        {
            bool gotLock = false;
            for (int i = 0; i < 10000; i++)
            {
                gotLock = false;
                try
                {
                    sl.Enter(ref gotLock);

                    sb.Append((i % 10).ToString());
                }
                finally
                {
                    // Only give up the lock if you actually acquired it
                    if (gotLock)
                        sl.Exit();
                }
            }
        };

        // Invoke 3 concurrent instances of the action above
        Parallel.Invoke(action, action, action);

        // Check/Show the results
        Console.WriteLine($"sb.Length = {sb.Length} (should be 30000)");

        Console.WriteLine($"number of occurrences of '5' in sb: {sb.ToString().Where(c => (c == '5')).Count()} (should be 3000)");

    }

    public static void Main()
    {
        //SpinLockDemo();
    }
}