using CodeMan.Seckill.HttpApi.Order.Extensions;
using CodeMan.Seckill.HttpApi.Order.service;

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
using AspNetCoreRateLimit;
using CodeMan.Seckill.Base.RabbitMq;
using CodeMan.Seckill.Service.service;
using Microsoft.AspNetCore.Http;

namespace CodeMan.Seckill.HttpApi.Order
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

            //需要存储速率限制计算器和ip规则
            services.AddMemoryCache();
            
            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));

            //注入计数器和规则存储
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CodeMan.Seckill.HttpApi.Order", Version = "v1" });
            });

            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<ISeckillGoodsService, SeckillGoodsService>();
            services.AddScoped<IRabbitProducer, RabbitProducer>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            // 配置（计数器密钥生成器）
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            services.ConfigureCors();
            services.ConfigureMySqlContext(Configuration);
            services.ConfigureRedisContext(Configuration);
            services.ConfigureRabbitContext(Configuration);
            services.ConfigureRepositoryWrapper();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CodeMan.Seckill.HttpApi.Order v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseIpRateLimiting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
