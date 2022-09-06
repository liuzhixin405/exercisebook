using IDCM.Contract.Business;
using IDCM.Contract.IBusiness;
using IDCM.Contract.WebApi.Extension;
using IDCM.Contract.WebApi.Orleans;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IDCM.Contract.WebApi
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

            services.AddControllers(o =>
            {
                o.Filters.Add<CustomExceptionFilter>();    //只能是实现了controllerbase的的才有效果,baseinfoGrains没有
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "IDCM.Contract.WebApi", Version = "v1" });
            });
            services.AddTransient<BaseInfoGrains>(); //orleans注册
            services.AddSingleton<IOrderBusiness,OrderBusiness>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "IDCM.Contract.WebApi v1"));
            }
            app.UseCors(x =>
            {
                x.AllowAnyOrigin()
                 .AllowAnyHeader()
                 .AllowAnyMethod()
                 .DisallowCredentials();
            });
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
