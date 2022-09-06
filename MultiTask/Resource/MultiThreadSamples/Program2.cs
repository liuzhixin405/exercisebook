using System;
using System.Threading;
/// <summary>
/// 基于信号量来解决线程的顺序问题
/// </summary>
class Example
{
    static ManualResetEvent mnlEvt = new ManualResetEvent(false);

    static AutoResetEvent evt1 = new AutoResetEvent(false);
    static AutoResetEvent evt2 = new AutoResetEvent(false);
    static AutoResetEvent evt3 = new AutoResetEvent(false);


    /// <summary>
    /// 基于信号来控制线程协作
    /// </summary>
    public static void TestManualResetEvent()
    {
        Thread th = new Thread(() =>
        {
            int n = 1;
            int result = 0;
            while (n <= 100)
            {
                // 延时模拟
                Thread.Sleep(20);
                result += n;
                n++;
            }
            Console.WriteLine("计算结果：{0}", result);
            mnlEvt.Set();
            // 发送信号后又马上切换为无信号状态
            //mnlEvt.Reset();
        });

        th.Start();

        Console.WriteLine("正在等待线程计算……");
        mnlEvt.WaitOne();
        Console.WriteLine("计算完毕！");
    }
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

    // 文件名
    static readonly string FileName = "demoFile.data";
    // 要写入文件的 9 个字节
    static readonly byte[] orgBuffer =
    {
            0x0C, 0x10, 0x02,
            0xE3, 0x71, 0xA2,
            0x13, 0xB8, 0x06
        };

    static AutoResetEvent[] writtenEvents = {
        new AutoResetEvent(false),
        new AutoResetEvent(false),
        new AutoResetEvent(false)
    };


    /// <summary>
    /// 多个线程同时写一个文件
    /// </summary>
    /// 
    public static void MutilThreadWriteFile()
    {
        for (int n = 0; n < 3; n++)
        {
            Thread th = new Thread((p) =>
            {
                // 先把要写的字节复制出来
                int currentCount = Convert.ToInt32(p);
                int copyIndex = currentCount * 3;
                byte[] tmpBuffer = new byte[3];
                Array.Copy(orgBuffer, copyIndex, tmpBuffer, 0, 3);
                // 打开文件流
                using (FileStream fs = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
                {
                    // 定位流的当前位置
                    fs.Seek(copyIndex, SeekOrigin.Begin);
                    // 写入数据
                    fs.Write(tmpBuffer, 0, tmpBuffer.Length);
                }
                // 发出信号
                writtenEvents[currentCount].Set();
            });
            // 标识为后台线程
            th.IsBackground = true;
            // 启动线程
            th.Start(n);
        }


        Console.WriteLine("等待所有线程完成文件写入……");
        WaitHandle.WaitAll(writtenEvents);
        Console.WriteLine("文件写入完成。读取数据");
        Console.WriteLine();

        using (FileStream fsin = new FileStream(FileName, FileMode.Open))
        {
            byte[] buffer = new byte[fsin.Length];
            fsin.Read(buffer, 0, buffer.Length);
            Console.WriteLine($"从文件读出来的字节：\n{BitConverter.ToString(buffer)}");
        }
    }

    /// <summary>
    /// 并行执行写入数据到多个文件
    /// </summary>
    public static void TestParallel()
    {
        string[] fileNames =
        {
            "demo_1_dx", "demo_2_dx", "demo_3_dx", "demo_4_dx",
            "demo_5_dx", "demo_6_dx", "demo_7_dx", "demo_8_dx"
        };

        Random rand = new Random();
        Parallel.ForEach(fileNames, (fn) =>
        {
            int len;
            byte[] data;
            lock (rand)
            {
                // 随机产生文件长度
                len = rand.Next(100, 90000);
                data = new byte[len];
                // 生成随机字节序列
                rand.NextBytes(data);
            }
            using (FileStream fs = new FileStream(fn, FileMode.Create))
            {
                fs.Write(data);
            }
            Console.WriteLine($"已向文件 {fn} 写入 {data.Length} 字节");
        });

    }

    //public static void Main()
    //{
    //    //TestManualResetEvent();
    //    TestParallel();
    //}
}