using Repository.Service.Extensions;
namespace OrderApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            // �������
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null; // ����ԭʼ����������ʹ���շ�������
                options.JsonSerializerOptions.WriteIndented = true; // ��ʽ�����
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles; // ����ѭ������
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
