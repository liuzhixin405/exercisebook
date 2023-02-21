using System.Threading.Tasks;

namespace ConsoleApp6
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
           //定时任务
            var result = await DoTaskWithTimeout(() =>
            {
                int i = 100000;
                while (i != 0)
                {
                    Console.WriteLine($"任务进行中:{DateTime.Now}, i:{i}");
                    i--;
                }
                return Task.CompletedTask;
            }, 1);
            Console.WriteLine($"任务完成状态:{result}");
        }

        public static async Task<bool> DoTaskWithTimeout(Func<Task> action, int timeoutSeconds)
        {
            var cts = new CancellationTokenSource();
             
            var timeout = Task.Delay(timeoutSeconds * 1000, cts.Token);

            var tcs = new TaskCompletionSource<bool>();
            var task = Task.Run( async() =>
            {
                // 这里是要执行的业务代码
                await action();
                tcs.SetResult(true);
            });
            cts.Token.Register(() =>
            {
                //TODO:任务取消后的业务
                    Console.WriteLine($"任务被取消了");
            });
            if (await Task.WhenAny(task, timeout) == timeout)//超时后
            {
                cts.Cancel();
                Console.WriteLine("任务超时");
                
                tcs.SetResult(false);
            }
            
            return tcs.Task.Result;
        }

    }
}