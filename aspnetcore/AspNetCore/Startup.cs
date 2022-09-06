using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.Config;
using AspNetCore.Extensions;
using AspNetCore.Helper;
using AspNetCore.MiddleFactory;
using AspNetCore.ThirdContainer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AspNetCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private static NameValueCollection _collection = new NameValueCollection();
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //services.AddControllersWithViews(options =>
            //{
            //    options.CacheProfiles.Add("Caching", new CacheProfile()
            //    {
            //        Duration = 120,
            //        Location = ResponseCacheLocation.Any,
            //        VaryByHeader = "cookie"
            //    });
            //    options.CacheProfiles.Add("NoCaching", new CacheProfile()
            //    {
            //        NoStore = true,
            //        Location = ResponseCacheLocation.None
            //    });

            //});        //等效responsecaching      Action加[ResponseCache(CacheProfileName = "Caching")]即可

            //services.AddTransient<FactoryActivatedMiddleware>();
            var aa = AppConfigurtaionServices.Configuration.GetSection("SchedulerConfig").GetChildren();
            foreach (var item in aa)
            {
                _collection.Add($"[{item.Key}]", $"\"{item.Value}\"");
                Console.WriteLine($"{item.Key}  +  {item.Value}");
            }
            var rr =_collection.Get("[quartz.serializer.type]");
            var cc = AppConfigurtaionServices.Configuration["SchedulerConfig:quartz.scheduler.instanceName"].ToString();

            //var res = Configuration.GetSection("SchedulerConfig").GetChildren();
            services.AddMemoryCache();
            services.AddDistributedMemoryCache();
            services.AddScoped<IMemoryCacheHelper, MemoryCacheHelper>();
            services.AddScoped<IDistributedCacheHelper, DistributedCacheHelper>();

            services.AddControllersWithViews();
        }
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseRequestCulture();
            //app.Run(async (context) => {
            //    await context.Response.WriteAsync($"Hello {CultureInfo.CurrentCulture.DisplayName}");
            //});
            //app.UseConventionalMiddleware();
            //app.UseFactoryActivatedMiddleware();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
