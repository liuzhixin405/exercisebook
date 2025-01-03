
namespace PluginApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var startup = new Startup(builder.Configuration);
            startup.ConfigureServices(builder.Services);
         

            var app = builder.Build();
            startup.Configure(app, builder.Environment);
           app.Run();
        }
    }
}
