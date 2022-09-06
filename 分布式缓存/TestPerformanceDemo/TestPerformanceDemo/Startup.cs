using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.ObjectPool;
using TestPerformanceDemo.Controllers;
using TestPerformanceDemo.ExtMiddleware;

namespace TestPerformanceDemo
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
            services.AddRazorPages();
            services.AddMvc(
                options =>
                {
                    options.CacheProfiles.Add("Default30", new CacheProfile() { 
                        Duration=30
                    });
                    options.SuppressAsyncSuffixInActionNames = false;
                }
                );
            services.AddSingleton<MyMemoryCache>();
            services.TryAddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();

            services.AddSingleton<ObjectPool<StringBuilder>>(serviceProvider => {
                var provider = serviceProvider.GetRequiredService<ObjectPoolProvider>();
                var policy = new StringBuilderPooledObjectPolicy();
                return provider.Create(policy);
                    
            });
            services.AddDistributedMemoryCache();
            services.AddDistributedSqlServerCache(o=>{
                o.ConnectionString = Configuration["DistCache_ConnectionString"];
                o.SchemaName = "dbo";
                o.TableName = "TestCache";
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime, IDistributedCache cache)
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
            lifetime.ApplicationStarted.Register(()=> {
                var currentUTC = DateTime.UtcNow.ToString();
                var cacheTest = "ƒ„∫√ ,’‚ «ª∫¥Ê....";
                byte[] encodedCurrentTimeUTC = Encoding.UTF8.GetBytes(currentUTC);
                byte[] encodedCurrentTest = Encoding.UTF8.GetBytes(cacheTest);
                var options = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(20));

                cache.Set("cachedTimeUTC", encodedCurrentTimeUTC, options);
                cache.Set("cachedTest", encodedCurrentTest, options);

            });
            app.UseMiddleware<BirthdayMiddleware>();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
