using ConsoleApp.Extends;
using Microsoft.AspNetCore.Hosting;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            new WebHostBuilder().UseKestrel().ConfigureServices(services =>
            services.AddHttpContextAccessor().AddSingleton<IFoobar, Foo>().AddSingleton<IFoobar, Bar>().AddSingleton<IFoobar, FoobarSelector>()
                .AddMvc()).Configure(app => app.UseMvc()).Build().Run();
        }
    }
    //http://localhost:5000/?source=MiniApp or http://localhost:5000/?source=App
    public class HomeController : Controller
    {
        private readonly IFoobar _foobar;
        public HomeController(IFoobar foobar) => _foobar = foobar;

        [HttpGet("/")]
        public Task Index(string source)
        {
            HttpContext.SetInvocationSource(source);
            return _foobar.InvokeAsync(HttpContext) ?? Task.CompletedTask;

        }
    }
}
