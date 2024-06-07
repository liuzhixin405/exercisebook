using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

internal class Program
{
    static void Main(string[] args)
    {
        int workerThreads, completionPortThreads;
        int cycleNumber = 10;
        ThreadPool.GetMaxThreads(out workerThreads, out completionPortThreads);
        Console.WriteLine($"Worker threads: {workerThreads}, Completion Port threads: {completionPortThreads}");

        string url = "http://localhost:5148/api/Order/TestTaskDelay";
        int requestCount = 1000;
        using HttpClient client = HttpClientFactory.Create();
        client.Timeout = TimeSpan.FromSeconds(60 * 2);

        for (int i = 0; i < cycleNumber; i++)
        {
            StartMethod(url, requestCount, client);
        }
        //Parallel.For(0, cycleNumber, i => { StartMethod(url, requestCount, client); }); //线程按照cycleNumber翻倍有点恐怖

        Console.WriteLine($"All threads have finished. failedCount: {failedCount}");
        Console.ReadLine();
    }

    private static void StartMethod(string url, int requestCount, HttpClient client)
    {
        Thread[] threads = new Thread[requestCount];
        CountdownEvent countdownEvent = new CountdownEvent(requestCount);

        for (int i = 0; i < requestCount; i++)
        {
            threads[i] = new Thread(() => ThreadMethod(client, url, countdownEvent));
        }

        // 确保所有线程准备就绪
        foreach (var thread in threads)
        {
            thread.Start();
        }

        // 等待所有线程准备好
        countdownEvent.Wait();

        Console.WriteLine("All threads are ready. Starting requests...");

        // 输出当前进程的线程数量
        int threadCount = Process.GetCurrentProcess().Threads.Count;
        Console.WriteLine($"Total threads: {threadCount}");

        // 等待所有线程完成
        foreach (var thread in threads)
        {
            thread.Join();
        }
    }

    static int failedCount = 0;
    static void ThreadMethod(HttpClient httpClient, string url, CountdownEvent countdownEvent)
    {      
        // 线程准备就绪，等待其他线程
        countdownEvent.Signal();
        countdownEvent.Wait();

        // 模拟并发请求
        try
        {
            Stopwatch watch = Stopwatch.StartNew();
            var result = httpClient.GetAsync(url).Result;
            watch.Stop();
            Console.WriteLine($"线程 {Thread.CurrentThread.ManagedThreadId} 请求完成,耗时: {watch.ElapsedMilliseconds} ms");
            if (result.IsSuccessStatusCode)
            {
                string content = result.Content.ReadAsStringAsync().Result;
                Console.WriteLine($"Response: {content}");
                //Thread.Sleep(60 * 1000);
                Console.WriteLine($"线程 {Thread.CurrentThread.ManagedThreadId} 休息好了,释放中");
            }
            else
            {
                failedCount++;
                Console.WriteLine($"Response: Error {result.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            failedCount++;
            Console.WriteLine($"Request failed: {ex.Message}");
        }
    }
}
