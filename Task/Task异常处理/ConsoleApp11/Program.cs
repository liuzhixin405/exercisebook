using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ConsoleApp11
{
    /// <summary>
    /// 应该是最优解
    /// </summary>
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                MyData myData = new MyData();
                Console.WriteLine("data is loaded: ");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    public class MyData
    {
        private static readonly TaskFactory _myTaskFactory =
           new TaskFactory(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);
        protected List<int>? Data { get; private set; }

        public MyData()
        {
            _myTaskFactory.StartNew(LoadDataAsync).Unwrap().ConfigureAwait(false).GetAwaiter().GetResult();
        }
        private async Task LoadDataAsync()
        {
            await Task.Delay(1000);
            //throw new Exception("Data loading failed!"); 
            Data = Enumerable.Range(1, 10).ToList();
        }
    }
}
