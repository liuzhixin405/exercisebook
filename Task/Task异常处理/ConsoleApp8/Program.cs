namespace ConsoleApp8
{
    internal class Program
    {
        private static AsyncLock  alock = new AsyncLock();
        static async Task Main(string[] args)
        {
            using(await alock.LockAsync())
            {
               //TODO:await 代码
            }
        }
       
    }
}