using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Orleans.Common;
using Orleans.Configuration;
using Orleans.Grains;
using Orleans.WebApi.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orleans.WebApi
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
            var silo = Configuration.GetSection("host").Get<SiloExOptions>();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Orleans.WebApi", Version = "v1" });
            });
            #region 第一种注入一 多客户端
            services.AddExtOrleansMultiClient(Configuration);
            #endregion

            #region 第二种注入 一个客户端
            var clientBuilder = new ClientBuilder()
                  .ConfigureLogging(logging => logging.AddConsole())
                  .UseStaticClustering(new System.Net.IPEndPoint(System.Net.IPAddress.Parse(silo.IpAddress), silo.GatewayPort))
                  .Configure<ClusterOptions>(options =>
                  {
                      options.ClusterId = silo.ClusterId;
                      options.ServiceId = silo.ServiceId;
                  })
                  .Configure<ClientMessagingOptions>(options => options.ResponseTimeout = TimeSpan.FromSeconds(30))
                  .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(IHelloA).Assembly).WithReferences());

            var client = clientBuilder.Build();
            client.Connect(async ex =>
            {
                await Task.Delay(TimeSpan.FromMilliseconds(300)).ConfigureAwait(false);
                return true;
            }).GetAwaiter().GetResult();
            services.AddSingleton(client);
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Globalvariable.ServiceLocatorInstance = app.ApplicationServices;
            //if (env.IsDevelopment())
            //{
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Orleans.WebApi v1"));
            //}

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
