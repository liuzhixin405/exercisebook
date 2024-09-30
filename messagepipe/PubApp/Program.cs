using MessagePipe;
namespace PubApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddMessagePipe().AddUdpInterprocess("127.0.0.1", 3215, options => { options.InstanceLifetime = InstanceLifetime.Singleton; });
            builder.Services.AddHostedService<PubMsgService>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
