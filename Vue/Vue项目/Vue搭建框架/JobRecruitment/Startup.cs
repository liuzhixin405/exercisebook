using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobRecruitment.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace JobRecruitment
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v6", new OpenApiInfo
                {
                    Title = "Web Shopping",
                    Version = "v1",
                    //Description = "����ACE����վ",
                    //TermsOfService = new Uri("http://www.baidu.com"),
                    //Contact = new OpenApiContact
                    //{
                    //    Name = "Ace",
                    //    Email = "2251610468@qq.com",
                    //    Url = new Uri("http://www.baidu.com")
                    //},
                    //License = new OpenApiLicense
                    //{
                    //    Name = "���֤",
                    //    Url = new Uri("http://www.baidu.com")
                    //}
                });
                // Ϊ Swagger JSON and UI����xml�ĵ�ע��·��
                // �ڴ�֮ǰҪ����Ŀ���������ã�����-��xml�ĵ��ļ���
                var xmlPath = AppDomain.CurrentDomain.BaseDirectory + "JobRecruitment.xml";
                c.IncludeXmlComments(xmlPath);
            });
            services.AddCors(a => a.AddPolicy("any", ap => ap.AllowAnyOrigin().AllowAnyMethod().AllowAnyMethod()));
            services.AddScoped<JobRecruitmentContext>();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v6/swagger.json", "Web Shopping V1");
            });
            app.UseRouting();

            app.UseAuthorization();

            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
