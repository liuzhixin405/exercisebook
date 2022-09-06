using System;
using System.Threading.Tasks;
namespace 第二版
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            MvcEngine mvcEngine = new FoobarMvcEngine();
            await mvcEngine.StartAsync(new Uri("http://localhost:80000"));
        }
    }

   

}
