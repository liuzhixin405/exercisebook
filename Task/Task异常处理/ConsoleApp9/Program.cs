namespace ConsoleApp9
{
    /// <summary>
    /// 构造函数加载初始化数据 
    /// </summary>
    internal class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                MyDataModel model = new MyDataModel();

                // 等待2秒，看看数据是否加载完成
                var timeoutTask = Task.Delay(2000);

                // 等待数据加载完成或者超时
                var completedTask = await Task.WhenAny(Task.Run(async () =>
                {
                    while (!model.IsDataLoaded)
                    {
                        await Task.Delay(100); // 等待数据加载完成
                    }
                }), timeoutTask);

                if (completedTask == timeoutTask)
                {
                    Console.WriteLine("Data loading failed!");
                }
                else if (model.IsDataLoaded)
                {
                    model.Data?.ForEach(x => Console.WriteLine(x));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }   

    public class MyDataModel
    {
        public List<int>? Data { get; private set; }
        public bool IsDataLoaded { get; private set; } = false;
        public MyDataModel()
        {
            //SafeFireAndForget(LoadDataAsync(), onCompleted: () => IsDataLoaded = true, onError: ex => Console.WriteLine(ex.Message)); 
            //LoadDataAsync().Await(onCompleted: () => IsDataLoaded = true, onError: ex => Console.WriteLine(ex.Message)); // 简化写法

            //第二种
            LoadDataAsync().ContinueWith(OnDataLoaded);
        }
        private bool OnDataLoaded(Task t)
        {
            if (t.IsFaulted)
            {
                Console.WriteLine(t.Exception.InnerException.Message);
                return false;
            }
            return IsDataLoaded = true;
        }
        private async Task LoadDataAsync()
        {
            await Task.Delay(1000);
            //throw new Exception("Data loading failed!"); //两中的异常都能捕获到,但是结果依然是isloaded=true , data=null
            Data = Enumerable.Range(1, 10).ToList();
        }
        private async void SafeFireAndForget(Task task, Action? onCompleted = null, Action<Exception>? onError = null)
        {
            try
            {
                await task;
                onCompleted?.Invoke();
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex);
                Console.WriteLine($"严重报错:{ex.Message}");
            }
        }
    }
    

    //简化写法
    static class TaskExtensions
    {
        public static async void Await(this Task task, Action? onCompleted = null, Action<Exception>? onError = null)
        {
            try
            {
                await task;
                onCompleted?.Invoke();
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex);
                Console.WriteLine($"严重报错:{ex.Message}");
            }
        }
    }

}
