using System.Diagnostics;
using System.Net.Http;

namespace ConsoleApp2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            int workerThreads, completionPortThreads;
            ThreadPool.GetMaxThreads(out workerThreads, out completionPortThreads);
            Console.WriteLine($"Worker threads: {workerThreads}, Completion Port threads: {completionPortThreads}");

            string url = "http://localhost:5148/api/Order/TestTaskDelay";
            int requestCount = 6000;
            using HttpClient client = HttpClientFactory.Create();
            client.Timeout = TimeSpan.FromSeconds(60 * 1);

            Task[] tasks = new Task[requestCount];
            for (int i = 0; i < requestCount; i++)
            {
                tasks[i] = Task.Factory.StartNew(() => ThreadMethod(client, url), TaskCreationOptions.LongRunning);
            }
            Task.WaitAll(tasks);

            //ParallelOptions options = new ParallelOptions();
            //options.MaxDegreeOfParallelism = 1000; // 设置最大并行度
            //Parallel.For(0, requestCount, options, i => { ThreadMethod(client, url); });
            Console.WriteLine("All threads have finished.");
            Console.ReadLine();
        }

        static void ThreadMethod(HttpClient httpClient, string url)
        {
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
                    Thread.Sleep(10 * 1000);
                    Console.WriteLine($"线程 {Thread.CurrentThread.ManagedThreadId} 休息好了,释放中");
                }
                else
                {
                    Console.WriteLine($"Response: Error {result.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Request failed: {ex.Message}");
            }
        }
    }

}