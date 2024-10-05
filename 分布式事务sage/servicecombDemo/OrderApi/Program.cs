using Repository.Service.Extensions;
namespace OrderApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            // 添加配置
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null; // 保留原始属性名（不使用驼峰命名）
                options.JsonSerializerOptions.WriteIndented = true; // 格式化输出
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles; // 忽略循环引用
            }); ;
            builder.Services.AddCors(options => { options.AddPolicy("CorsPolicy", builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); }); });
            builder.Services.AddInternalOrderServices();
            builder.Services.AddMysqlServices(Repository.Service.Enums.ServiceType.Order, builder.Configuration);
            builder.Services.AddInternalSwagger("Order API","v1");
            var app = builder.Build();
            app.UseInternalSwagger("Order API", "v1");
            app.UseCors("CorsPolicy");
            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
