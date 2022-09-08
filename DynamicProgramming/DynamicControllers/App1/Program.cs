using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace App
{
public class Program
{
    public static void Main()
    {

        Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(web => web
                .ConfigureServices(svcs => svcs
                    .AddSingleton<ICompiler, Compiler>()
                    .AddSingleton<DynamicActionProvider>()
                    .AddSingleton<DynamicChangeTokenProvider>()
                    .AddSingleton<IActionDescriptorProvider>(provider => provider.GetRequiredService<DynamicActionProvider>())
                    .AddSingleton<IActionDescriptorChangeProvider>(provider => provider.GetRequiredService<DynamicChangeTokenProvider>())
                    .AddRouting().AddControllersWithViews())
                .Configure(app => app
                    .UseRouting()
                    .UseEndpoints(endpoints => endpoints.MapControllerRoute(
                        name: default,
                        pattern: "{controller}/{action}"
                        ))))
            .Build()
            .Run();
    }
}
}
