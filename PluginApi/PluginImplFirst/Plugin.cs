using IPluginLibrary;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace PluginImplFirst
{
    public class Plugin: IWebApiPlugin
    {
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // 在这里添加插件的服务配置
            // 添加插件的中间件配置
            if (env.EnvironmentName.Equals("Development"))
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Plugin API");
                    c.RoutePrefix = "swagger";
                });
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("/plugin", async context =>
                {
                    await context.Response.WriteAsync("Plugin response");
                }).WithName("xxx").WithGroupName("aaa").WithOpenApi()
                
                ;
            });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // 在这里添加插件的服务配置

            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Plugin API", Version = "v1" });
                c.DocInclusionPredicate((docName, apiDesc) =>
                {
                    // 如果文档名称以 "v" 开头，说明是其他文档
                    return docName.StartsWith("v");
                });
            });
        }
    }
}
