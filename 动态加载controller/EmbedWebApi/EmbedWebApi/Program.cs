using Embed.WebApi;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

//[assembly: ApplicationPart("Embed.WebApi")]
namespace EmbedWebApi
{
    internal class Program
    {
        static void Main(string[] args)
        {

            var services = new ServiceCollection();
            services.TryAddTransient<IInitTest, InitTest>();
            var serviceProvider = services.BuildServiceProvider();

            var initTest = serviceProvider.GetRequiredService<IInitTest>();

            initTest.Init();

            Console.Read();
        }
    }
}