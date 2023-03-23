
using AspectCore.Extensions.DependencyInjection;

namespace AspNetCoreAOP
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers().AddControllersAsServices();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Host.UseServiceProviderFactory(new DynamicProxyServiceProviderFactory());
            builder.Services.ConfigureDynamicProxy(o =>{ 
            //添加aop的配置
            //该项目用attribute所以无需配置
           
            });
            //自定义实现aop请翻看以前的例子,或者查看这个引用包源码

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}