using ConsoleAppTwo.Extends;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleAppTwo
{
    class Program
    {
        static void Main(string[] args)
        {
            new WebHostBuilder().UseKestrel().ConfigureServices(services =>
            services.AddHttpContextAccessor().AddSingleton<IFoobarProvider, FoobarProvider>()
                .AddMvc()).Configure(app => app.UseMvc()).Build().Run();
        }
    }
    //http://localhost:5000/?source=MiniApp or http://localhost:5000/?source=App
    public class HomeController : Controller
    {
        private readonly IFoobarProvider _provider;
        public HomeController(IFoobarProvider provider) => _provider = provider;

        [HttpGet("/")]
        public Task Index(string source)
        {
            HttpContext.SetInvocationSource(source);
            return _provider.GetService()?.InvokeAsync(HttpContext) ?? Task.CompletedTask;

        }
    }
}
