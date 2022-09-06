using IDCM.Contract.WebApi.Extension;
using IDCM.Contract.WebApi.Orleans;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IDCM.Contract.Core.Extension;

namespace IDCM.Contract.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
           
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls("https://localhost:5002");
                    webBuilder.UseStartup<Startup>();
                }).UseExtOrleans(builder =>
                {
                    builder.ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(BaseInfoGrains).Assembly).WithReferences())
                    .AddIncomingGrainCallFilter<ExceptionCallFilter>();
                    ;

                });
    }
}
