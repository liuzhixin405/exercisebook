using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Reflection;

namespace ControllerWebApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddSingleton<ICompiler, Compiler>()
                    .AddSingleton<DynamicChangeTokenProvider>()
                    .AddSingleton<IActionDescriptorChangeProvider>(provider => provider.GetRequiredService<DynamicChangeTokenProvider>()); //��̬
            builder.Services.AddControllers();
            //builder.Services.AddControllers().AddApplicationPart(Assembly.LoadFile("G:\\Users\\GitHub\\exercisebook\\��̬����controller\\EmbedWebApi\\ControllerLibrary\\bin\\Debug\\net6.0\\ControllerLibrary.dll")); //д��

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
           
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