using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Pandora.Cigfi.Web
{

    public static class Program
    {
        public static readonly Version CurrentVersion = typeof(Startup).Assembly.GetName().Version;

        public static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            #region 初始化 

            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                 .AddEnvironmentVariables()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                 .AddJsonFile($"appsettings.{env}.json", optional: false)

              .AddCommandLine(args) 
                .Build();
            /*for (int i = 0; i < 3; i++)
            {
                if (string.IsNullOrEmpty(config["ConnectionStrings:DataCenter"]))
                {
                    config.Reload();
                }
                else
                {
                    break;
                }
            }*/
            //Debug模式会直接写到控制台，发布模式写到文件

            #endregion
            Cigfi.Common.CommonUtils.Configuration = config;
            CreateHostBuilder(config, args).Build().Run();

        }
        public static IHostBuilder CreateHostBuilder(IConfiguration config, string[] args) =>
           Host.CreateDefaultBuilder(args)
               .ConfigureWebHostDefaults(webBuilder =>
               {
                   webBuilder
                   .UseConfiguration(config)
                   .UseStartup<Startup>()
                   .UseSerilog((ctx, cfg) => cfg
                    .ReadFrom.Configuration(ctx.Configuration));
               });
    }

   
}
