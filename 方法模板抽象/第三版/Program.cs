using ClassLibrary;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
namespace 第三版
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            MvcEngine mvcEngine = new MvcEngine(new SingletonWebListener(),new SingletonControllerActivator(),new SingletonControllerExecutor(),new SingletonViewRender());
            await mvcEngine.StartAsync(new Uri("http://localhost:8000"));
        }
    }

   
}
