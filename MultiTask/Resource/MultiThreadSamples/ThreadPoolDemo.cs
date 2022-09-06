using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiThreadSamples
{
    public class ThreadPoolDemo
    {
        public static void TestDemo1()
        {
            int i = 0;
            int j = 0;
            //前面是辅助(也就是所谓的工作者)线程，后面是I/O线程
            ThreadPool.GetMaxThreads(out i, out j);
            Console.WriteLine(i.ToString() + "   " + j.ToString()); //默认都是1000

            //获取空闲线程，由于现在没有使用异步线程，所以为空
            ThreadPool.GetAvailableThreads(out i, out j);
            Console.WriteLine(i.ToString() + "   " + j.ToString()); //默认都是1000
        }

        static void RunWorkerThread1(object state)
        {
            Console.WriteLine("RunWorkerThread开始工作");
            Console.WriteLine("工作者线程启动成功!");
        }

        static void RunWorkerThread2(object obj)
        {
            Thread.Sleep(200);
            Console.WriteLine("线程池线程开始!");
            User? p = obj as User;
            Console.WriteLine($"name={p!.Name} Age={p!.Age}");
        }

        //定义委托
        delegate string MyDelegate(User user);

        static string GetString(User user)
        {
            Console.WriteLine("我是不是线程池线程" + Thread.CurrentThread.IsThreadPoolThread);
            Thread.Sleep(2000);
            return string.Format("我是{0}，今年{1}岁!", user.Name, user.Age);
        }

        public static void TestDemo2()
        {
            //工作者线程最大数目，I/O线程的最大数目
            ThreadPool.SetMaxThreads(1000, 1000);
            //启动工作者线程
            ThreadPool.QueueUserWorkItem(new WaitCallback(RunWorkerThread1!));
            Console.ReadKey();
        }

        public static void TestDemo3()
        {
            User u = new User(10001L, "Gerry", 30);
            //启动工作者线程
            ThreadPool.QueueUserWorkItem(new WaitCallback(RunWorkerThread2!), u);
            Console.ReadKey();
        }

        public static void TestDemo4()
        {
            //建立委托
            MyDelegate myDelegate = new MyDelegate(GetString);
            User u = new User(10001L, "Gerry", 30);
            var task = Task.Run(() => { return myDelegate.Invoke(u); });
            Console.WriteLine("主线程继续工作!");
            //注意获取返回值的方式
            string data = task.Result;
            Console.WriteLine($"返回结果为:{data}");
        }

        /// <summary>
        /// 多执行几次看结果，观察线程释放重用
        /// </summary>
        public static void TestDemo5()
        {
            Console.WriteLine($"Main 方法开始，ThreadId：{Thread.CurrentThread.ManagedThreadId}，DateTime：{DateTime.Now.ToLongTimeString()}\n");

            ThreadPool.QueueUserWorkItem(t => { Console.WriteLine($"张三，任务处理完成。ThreadId:{Thread.CurrentThread.ManagedThreadId}"); });
            ThreadPool.QueueUserWorkItem(t => { Console.WriteLine($"李四，任务处理完成。ThreadId:{Thread.CurrentThread.ManagedThreadId}"); });
            ThreadPool.QueueUserWorkItem(t => { Console.WriteLine($"王五，任务处理完成。ThreadId:{Thread.CurrentThread.ManagedThreadId}"); });
            ThreadPool.QueueUserWorkItem(t => { Console.WriteLine($"赵六，任务处理完成。ThreadId:{Thread.CurrentThread.ManagedThreadId}"); });

            Thread.Sleep(1000); Console.WriteLine();

            ThreadPool.QueueUserWorkItem(t => { Console.WriteLine($"张三，任务处理完成。ThreadId:{Thread.CurrentThread.ManagedThreadId}"); });
            ThreadPool.QueueUserWorkItem(t => { Console.WriteLine($"李四，任务处理完成。ThreadId:{Thread.CurrentThread.ManagedThreadId}"); });
            ThreadPool.QueueUserWorkItem(t => { Console.WriteLine($"王五，任务处理完成。ThreadId:{Thread.CurrentThread.ManagedThreadId}"); });
            ThreadPool.QueueUserWorkItem(t => { Console.WriteLine($"赵六，任务处理完成。ThreadId:{Thread.CurrentThread.ManagedThreadId}"); });

            Thread.Sleep(1000); Console.WriteLine();

            ThreadPool.QueueUserWorkItem(t => { Console.WriteLine($"张三，任务处理完成。ThreadId:{Thread.CurrentThread.ManagedThreadId}"); });
            ThreadPool.QueueUserWorkItem(t => { Console.WriteLine($"李四，任务处理完成。ThreadId:{Thread.CurrentThread.ManagedThreadId}"); });
            ThreadPool.QueueUserWorkItem(t => { Console.WriteLine($"王五，任务处理完成。ThreadId:{Thread.CurrentThread.ManagedThreadId}"); });
            ThreadPool.QueueUserWorkItem(t => { Console.WriteLine($"赵六，任务处理完成。ThreadId:{Thread.CurrentThread.ManagedThreadId}\n"); });

            Thread.Sleep(1000);

            Console.WriteLine($"Main 方法结束，ThreadId：{Thread.CurrentThread.ManagedThreadId}，DateTime：{DateTime.Now.ToLongTimeString()}");

            Console.ReadLine();
        }

        /// <summary>
        /// 一个比较耗时的方法,循环1000W次
        /// </summary>
        /// <param name="name"></param>
        public static void DoSomethingLong(string name)
        {
            int iResult = 0;
            for (int i = 0; i < 1000000000; i++)
            {
                iResult += i;
            }

            Console.WriteLine($"******{name}*****{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")}****{Thread.CurrentThread.ManagedThreadId}****");
        }

        /// <summary>
        /// 
        /// </summary>
        public static void TestDemo6()
        {
            //用来控制线程等待,false默认为关闭状态
            ManualResetEvent mre = new ManualResetEvent(false);
            ThreadPool.QueueUserWorkItem(p =>
            {
                DoSomethingLong("控制线程等待");
                Console.WriteLine($"线程池线程，带参数,{Thread.CurrentThread.ManagedThreadId}");
                //通知线程，修改信号为true
                mre.Set();
            });

            //阻止当前线程，直到等到信号为true在继续执行
            mre.WaitOne();

            //关闭线程，相当于设置成false
            mre.Reset();
            Console.WriteLine("信号被关闭了");

            ThreadPool.QueueUserWorkItem(p =>
            {
                Console.WriteLine("再次等待");
                Thread.Sleep(2000);
                mre.Set();
            });

            mre.WaitOne();
            Console.WriteLine("主线程结束");
        }


        static AutoResetEvent evt1 = new AutoResetEvent(false);
        static AutoResetEvent evt2 = new AutoResetEvent(false);
        static AutoResetEvent evt3 = new AutoResetEvent(false);

        /// <summary>
        /// 等待线程信号——AutoResetEvent
        /// </summary>
        public static void TestAutoResetEvent()
        {
            Thread th1 = new Thread(() =>
            {
                Console.WriteLine("正在进行第一阶段……");
                Thread.Sleep(2000);
                Console.WriteLine("第一阶段处理完成！");
                // 发送信号
                evt1.Set();
            });

            Thread th2 = new Thread(() =>
            {
                // 等待第一阶段完成
                evt1.WaitOne();
                Console.WriteLine("正在进行第二阶段……");
                Thread.Sleep(2000);
                Console.WriteLine("第二阶段处理完成！");
                // 发出信号
                evt2.Set();
            });

            Thread th3 = new Thread(() =>
            {
                // 等待第二阶段完成
                evt2.WaitOne();
                Console.WriteLine("正在进行第三阶段……");
                Thread.Sleep(2000);
                Console.WriteLine("第三阶段处理完成！");
                // 发送信号
                evt3.Set();
            });

            th1.Start();
            th2.Start();
            th3.Start();

            evt3.WaitOne();
            Console.WriteLine("\n已完成所有操作。");
        }


        /// <summary>
        /// 并行编程
        /// </summary>
        public static void TestDemo7()
        {
            //并行编程 
            Console.WriteLine($"*************Parallel start********{Thread.CurrentThread.ManagedThreadId}****");
            //一次性执行1个或多个线程，效果类似：Task WaitAll，只不过Parallel的主线程也参与了计算
            Parallel.Invoke(
                () => { DoSomethingLong("Parallel-1`1"); },
                () => { DoSomethingLong("Parallel-1`2"); },
                () => { DoSomethingLong("Parallel-1`3"); },
                () => { DoSomethingLong("Parallel-1`4"); },
                () => { DoSomethingLong("Parallel-1`5"); });

            //定义要执行的线程数量
            Parallel.For(0, 5, t =>
            {
                int a = t;
                DoSomethingLong($"Parallel-2`{a}");

            });
            Console.WriteLine("=====================================================================");
            {
                ParallelOptions options = new ParallelOptions()
                {
                    MaxDegreeOfParallelism = 3//执行线程的最大并发数量,执行完成一个，就接着开启一个
                };

                //遍历集合，根据集合数量执行线程数量
                Parallel.ForEach(new List<string> { "a", "b", "c", "d", "e", "f", "g" }, options, t =>
                {
                    DoSomethingLong($"Parallel-3`{t}");
                });
            }

            {
                ParallelOptions options = new ParallelOptions()
                {
                    MaxDegreeOfParallelism = 3//执行线程的最大并发数量,执行完成一个，就接着开启一个
                };

                //遍历集合，根据集合数量执行线程数量
                Parallel.ForEach(new List<string> { "a", "b", "c", "d", "e", "f", "g" }, options, (t, status) =>

                {
                    //status.Break();//这一次结束。
                    //status.Stop();//整个Parallel结束掉，Break和Stop不可以共存
                    DoSomethingLong($"Parallel-4`{t}");
                });
            }

            Console.WriteLine("*************Parallel end************");
            Console.ReadLine();

        }

        public static void Main()
        {
            //TestDemo1();
            //TestDemo2();
            //TestDemo3();
            //TestDemo4();
            //TestDemo5();
            //TestDemo6();
            //TestAutoResetEvent();
            TestDemo7();
        }


        /// <summary>
        /// 定义用户对象类
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Name"></param>
        /// <param name="Age"></param>
        record User(long Id, string Name, int Age);
    }
}
