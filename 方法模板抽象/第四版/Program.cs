using System;
using System.Threading.Tasks;

namespace 第四版
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var adderss = new Uri("http://localhost:8000");
            var engine = new MvcEngine(new FoobarEngineFactory());
            await engine.StartAsync(adderss);
        }
    }
}
