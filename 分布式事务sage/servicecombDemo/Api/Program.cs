using Repository.Service.Extensions;
namespace Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddInternalSwagger("Api", "v1");
            builder.Services.AddTransient<ApiServicecs>();
            builder.Services.AddHttpClient();
            var app = builder.Build();
            app.UseInternalSwagger("Api", "v1");
            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
