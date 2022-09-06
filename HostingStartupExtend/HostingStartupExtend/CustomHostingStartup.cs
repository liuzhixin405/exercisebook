using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[assembly: HostingStartup(typeof(HostingStartupExtend.CustomHostingStartup))]
namespace HostingStartupExtend
{
    public class CustomHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            Console.WriteLine("This is CustomHostingStartup Invoke");

            //有IWebHostBuilder，一切都可以做。。
            #region MyRegion


            builder.Configure(app =>
            {
                app.Use(next =>
                {
                    Console.WriteLine("This is CustomHostingStartup-Middleware  Init");
                    return new RequestDelegate(
                        async context =>
                        {
                            Console.WriteLine("This is CustomHostingStartup-Middleware start");
                            await next.Invoke(context);
                            Console.WriteLine("This is CustomHostingStartup-Middleware end");
                        });
                });
            });//甚至来个中间件
            #endregion
        }
    }
}
