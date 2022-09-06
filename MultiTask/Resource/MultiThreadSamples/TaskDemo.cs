using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiThreadSamples
{
    public class TaskDemo
    {
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

        public static void TaskDemo1()
        {
            //线程容器
            List<Task> taskList = new List<Task>();
            Stopwatch watch = new Stopwatch();
            watch.Start();
            Console.WriteLine("************Task Begin**************");

            //启动5个线程
            for (int i = 0; i < 5; i++)
            {
                string name = $"Task:{i}";
                Task task = Task.Factory.StartNew(() =>
                {
                    DoSomethingLong(name);
                });

                taskList.Add(task);
            }

            //回调形式的，任意一个完成后执行的后续动作
            Task any = Task.Factory.ContinueWhenAny(taskList.ToArray(), task =>
            {
                Console.WriteLine("ContinueWhenAny");
            });

            //回调形式的，全部任务完成后执行的后续动作
            Task all = Task.Factory.ContinueWhenAll(taskList.ToArray(), tasks =>
            {
                Console.WriteLine($"ContinueWhenAll,线程数：{tasks.Length}");
            });

            //把两个回调也放到容器里面，包含回调一起等待
            taskList.Add(any);
            taskList.Add(all);

            //WaitAny:线程等待，当前线程等待其中一个线程完成
            Task.WaitAny(taskList.ToArray());
            Console.WriteLine("WaitAny");

            //WaitAll:线程等待，当前线程等待所有线程的完成
            Task.WaitAll(taskList.ToArray());
            Console.WriteLine("WaitAll");

            Console.WriteLine($"************Task End**************{watch.ElapsedMilliseconds},{Thread.CurrentThread.ManagedThreadId}");

        }

        public static void Main()
        {
            TaskDemo1();
        }
    }
}
