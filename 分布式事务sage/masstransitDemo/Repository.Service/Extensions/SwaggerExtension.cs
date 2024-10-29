using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Service.Extensions
{
    public static class SwaggerExtension
    {
        public static IServiceCollection AddInternalSwagger(this IServiceCollection services,string title,string version)
        {
            // 添加Swagger服务
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = title, Version = version});
            });
            return services;
        }

        public static WebApplication UseInternalSwagger(this WebApplication app,string title,string version)
        {
            // 配置HTTP请求管道
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{title} {version}"));
            }
            return app;
        }
    }
}
