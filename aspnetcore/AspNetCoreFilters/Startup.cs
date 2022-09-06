using AspNetCoreFilters.AttributeModel;
using AspNetCoreFilters.Filters;
using AspNetCoreFilters.OptionsModel;
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

namespace AspNetCoreFilters
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
            services.Configure<PositionOptions>(Configuration.GetSection("Position"));
            services.AddScoped<MyActionFilterAttribute>();
            services.AddScoped<AddHeaderResultServiceFilter>();

            //services.AddControllers();       api 
            services.AddControllersWithViews(options => {         //mvc测试替换掉了api
                options.Filters.Add<UnprocessableResultFilter>();
                //o.Filters.Add<MySampleActionFilter>(); 同步
                options.Filters.Add<SampleAsyncActionFilter>();    //异步

                options.Filters.Add(new AddHeaderAttribute("GlobalAddHeader",
          "Result filter added to MvcOptions.Filters"));         // An instance       全局过滤器       
                options.Filters.Add<CustomAuthorizeFilter>();
            });
            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AspNetCoreFilters", Version = "v1" });
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseSwagger();
                //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AspNetCoreFilters v1"));
            }

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
