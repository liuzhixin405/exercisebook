namespace ConsoleApp9
{
    /// <summary>
    /// 构造函数加载初始化数据 
    /// </summary>
    internal class Program
    {
        static Task Main(string[] args)
        {
            try
            {
                MyDataModel model = new MyDataModel();

                while (!model.IsDataLoaded)
                {
                    Console.WriteLine("Loading data..."); //这样有点危险，如果加载数据失败，程序会陷入死循环
                    Thread.Sleep(100); //等待数据加载完成
                }


                model.Data.ForEach(x => Console.WriteLine(x)); //ForEach性能最低
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Task.CompletedTask;
          
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
            }
            return IsDataLoaded = true;
        }
        private async Task LoadDataAsync()
        {
            await Task.Delay(1000);
            throw new Exception("Data loading failed!"); //两中的异常都能捕获到,但是结果依然是isloaded=true , data=null
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
