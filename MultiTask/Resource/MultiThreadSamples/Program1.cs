using System.Diagnostics;

/// <summary>
/// 线程池和异步
/// </summary>
class Program
{
    private static void TaskFunc(string name = "")
    {
        //获取正在运行的线程
        Thread thread = Thread.CurrentThread;
        //设置线程的名字
        //thread.Name = name;//只能设置一次
        Console.WriteLine(thread.Name);
        Console.WriteLine(name);
        //获取当前线程的唯一标识符
        int id = thread.ManagedThreadId;
        //获取当前线程的状态
        System.Threading.ThreadState state = thread.ThreadState;
        //获取当前线程的优先级
        ThreadPriority priority = thread.Priority;
        string strMsg = string.Format("Thread ID:{0}\n" + "Thread Name:{1}\n" +
            "Thread State:{2}\n" + "Thread Priority:{3}\n", id, thread.Name,
            state, priority);
        Console.WriteLine(strMsg);
        //执行耗时间耗资源的任务
        Console.WriteLine(DateTime.Now.Ticks);
        Console.WriteLine("==============================================");
    }
    

    private static void Test1()
    {
        //同步延时，阻塞主线程
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        Thread.Sleep(500);
        stopwatch.Stop();
        Console.WriteLine("stopwatch：" + stopwatch.ElapsedMilliseconds);
        TaskFunc();
        //异步延时，不阻塞主线程
        Stopwatch stopwatch2 = new Stopwatch();
        stopwatch2.Start();
        var t1 = Task.Delay(500).ContinueWith(t =>
        {
            stopwatch2.Stop();
            Console.WriteLine("stopwatch2：" + stopwatch2.ElapsedMilliseconds);
        });
        TaskFunc();
        //同步+异步延时，不阻塞主线程
        Stopwatch stopwatch3 = new Stopwatch();
        stopwatch3.Start();
        var t2 = Task.Run(() =>
        {
            Thread.Sleep(500);
            stopwatch3.Stop();
            Console.WriteLine("stopwatch3：" + stopwatch3.ElapsedMilliseconds);
            TaskFunc();
        });
        Task.WaitAll(t1, t2);
        Console.WriteLine("主线程结束");
    }

    /// <summary>
    /// 通过判断线程状态来控制线程最大运行数
    /// </summary>
    private static void Test2()
    {
        var maxCount = 20;
        List<int> list = new List<int>();
        for (int i = 0; i < 100; i++)
        {
            list.Add(i);
        }
        Action<int> action = i =>
        {
            TaskFunc();
            Thread.Sleep(10);
        };
        List<Task> taskList = new List<Task>();
        foreach (var i in list)
        {
            int k = i;
            taskList.Add(Task.Run(() => action.Invoke(k)));
            if (taskList.Count > maxCount)
            {
                Task.WaitAny(taskList.ToArray());
                taskList = taskList.Where(t => t.Status != TaskStatus.RanToCompletion).ToList();
                Console.WriteLine("运行中的任务数：" + taskList.Count);
            }
        }
        //异步等待其全部执行完毕，不阻塞线程
        Task wTask = Task.WhenAll(taskList.ToArray());
        //wTask.ContinueWith()...
        //死等线程全部执行完毕，阻塞后面的线程
        Task.WaitAll(taskList.ToArray());
        //Task.WaitAll()和Task.WhenAll()区别一个阻塞线程，一个不阻塞
        Console.WriteLine("主线程结束");
    }

    private static void Test3()
    {
        //for (int i = 0; i < 100; i++)
        //{
        //    new Thread(() => { Thread.Sleep(100000); }).Start();
        //}

        for (int i = 0; i < 100; i++)
        {

            Task.Run(() => { Thread.Sleep(100000); });
        }
        Console.ReadLine();
    }
    /// <summary>
    /// ParallelOptions 控制并发数量
    /// </summary>
    public static void Test4()
    {
        //state.Break()和state.Stop() 都不推荐用，异常情况处理较麻烦
        ParallelOptions parallelOptions = new ParallelOptions();
        parallelOptions.MaxDegreeOfParallelism = 2;//控制并发数量
        Parallel.For(1, 12, parallelOptions, (i, state) =>
        {
            //state.Stop();/*
            //调用 Stop 方法指示尚未开始的循环的任何迭代都无需运行。 它可以有效地取消循环的任何其他迭代。 但是，它不会停止已经开始执行的任何迭代。
            //调用 Stop 方法会导致此 IsStopped 属性返回到 true 仍在执行的循环的任何迭代。 这对于长时间运行的迭代特别有用，它可以检查 IsStopped 属性并在其值为时提前退出 true 。
            //Stop 通常在基于搜索的算法中使用，在找到结果后，不需要执行其他迭代。
            //state.Break();
            //Break 指示应运行当前迭代之后的任何迭代。 它可以有效地取消循环的任何其他迭代。 但是，它不会停止已经开始执行的任何迭代。 例如，如果 Break 是从从0到1000的并行循环的第100迭代调用的，则所有小于100的迭代仍应运行，但不会执行从101到1000的迭代。
            //对于可能已在执行的长时间运行的迭代， Break LowestBreakIteration 如果当前索引小于的当前值，则将属性设置为当前迭代的索引 LowestBreakIteration 。 若要停止其索引大于从争用执行的最低中断迭代的迭代，应执行以下操作：
            //检查属性是否 ShouldExitCurrentIteration 为 true 。
            //如果其索引大于属性值，则从迭代退出 LowestBreakIteration 。
            //说明如示例所示。
            //Break 通常在基于搜索的算法中采用，其中排序在数据源中存在。
            TaskFunc();
        });

    }

    //static void Main(string[] args)
    //{
    //    // Test1();
    //    // Test2();
    //    //Test3();
    //    //Test4();
    //}
}