using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Profile;
using FXH.Common.DapperService;
using FXH.Common.Logger;
using FXH.Common.Orm.MongoService;
using FXH.Common.Oss.AliOss;
using FXH.Common.Oss.AliOss.Implement;
using FXH.Common.Oss.AliOss.Interface;
using FXH.Redis.Extensions;
using FXH.Redis.Extensions.Configuration;
using FXH.Redis.Extensions.Serializer;
using FXH.Web.Extensions;
using FXH.Web.Extensions.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.WebEncoders;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Pandora.Cigfi.Common;
using Pandora.Cigfi.Common.AliyunCore.Utils;
using Pandora.Cigfi.Core.MiddlewareExtension.Extension;
using Pandora.Cigfi.Models.Consts;
using Pandora.Cigfi.Models.Sys;
using Pandora.Cigfi.IServices.Cigfi;
using Pandora.Cigfi.Services.Cigfi;

using Pandora.Cigfi.Web.Common;
using Pandora.Cigfi.Web.ExceptionHandler;
using Pandora.Cigfi.Web.Models;
using Refit;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading;
using System.Threading.Tasks;
using Pandora.Cigfi.Services.Cigfi.Invitation;

namespace Pandora.Cigfi.Web
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
            services.AddSingleton<IConfigurationManager, ConfigurationManager>();
            //配置阿里云oss
            services.Configure<AliOssConfig>(Configuration.GetSection("AliOssConfig"));
            services.AddSingleton<IAliOssService, AliOssService>();

            services.AddSingleton<GreenTestUtil>((x) => new GreenTestUtil(Configuration.GetSection("aliyun:regionId").Value, Configuration.GetSection("aliyun:accessKeyId").Value, Configuration.GetSection("aliyun:accessKeySecret").Value));


            services.AddAutoMapper(excludeDllNames:new List<string> { ".Views.dll" });
            //注册HttpClient服务
            services.AddHttpClient();


            var afsConfig = Configuration.GetSection("aliyun_afs").Get<AliyunAFSConfig>();
            IClientProfile profile = DefaultProfile.GetProfile(afsConfig.regionId, afsConfig.accessKeyId, afsConfig.accessKeySecret);
            DefaultProfile.AddEndpoint(afsConfig.regionId, afsConfig.regionId, "afs", "afs.aliyuncs.com");
            services.AddSingleton(afsConfig);
            services.AddScoped<IAcsClient>(provider => new DefaultAcsClient(profile));


            #region 日志配置
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
               .CreateLogger();
            #endregion

            //注入自己的HttpContext
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
           
            services.AddScoped<RequestCheckHelper>();
            
            services.AddMyHttpContextAccessor();

            services.AddDistributedMemoryCache();
            //添加Session 服务 
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(5);
                options.Cookie.HttpOnly = true;

            });
            //部分系统配置
            services.Configure<SystemSetting>(Configuration.GetSection("DataCenter:SystemSetting"));

            services.AddControllersWithViews(options => {
                options.SuppressAsyncSuffixInActionNames = false;
            });
          
            #region 数据库配置
            var connectionString = string.IsNullOrEmpty(Configuration["MYSQL_CONNSTR"]) ? Configuration["ConnectionStrings:DataCenter"] : Configuration["MYSQL_CONNSTR"];

            // connectionString = "server=122.228.200.88;port=53592;user=fxhcms;password=FXH6HAGYAcd56871Bzsdadsp#$1329087;database=datacenter;SslMode = none;";

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new Exception("ConnectionStrings Lose");
            services.AddDapper(options =>
            {
                options.ConnectionString = connectionString;
                options.DatabaseType = DatabaseType.MySql;
            });
            #endregion

            #region redis 缓存配置
            RedisConfiguration redisConfiguration;
            if (string.IsNullOrEmpty(Configuration["REDIS_CONNSTR"]))
            {
                redisConfiguration = Configuration.GetSection("Redis").Get<RedisConfiguration>();
            }
            else
            {
                redisConfiguration = new RedisConfiguration { Hosts = Configuration["REDIS_CONNSTR"].Split(',').Select(c => c.Split(':')).Where(c => c.Length == 2).Select(c => new RedisHost { Host = c[0], Port = int.Parse(c[1]) }).ToArray() };
                if (!string.IsNullOrEmpty(Configuration["REDIS_PASSWORD"])) redisConfiguration.Password = Configuration["REDIS_PASSWORD"];
            }

            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
            //services.AddRedis(() => redisConfiguration, Redis.Extensions.Serializer.SerializerEnum.PROTOBUF);

            services.AddSingleton<IRedisCache>(obj =>
            {
                var serializer = new MsgPackSerializer();
                var connection = new PooledConnectionMultiplexer(redisConfiguration.ConfigurationOptions);
                return new RedisCache(obj.GetRequiredService<ILogger<RedisCache>>(), connection, redisConfiguration, serializer);
            });

            //services.AddSingleton(obj =>
            //{
            //    return new FXH.Api.Utils.RedisCache(redisConfiguration.ConfigurationOptions,
            //        8,
            //        0);
            //});

            //services.AddSingleton<IRedisCache>(obj =>
            //{
            //    // JsonSerializerSettings setting = new JsonSerializerSettings();
            //    //var serializer = new Redis.Extensions.Serializer.JsonSerializer(setting);
            //    var serializer = new MsgPackSerializer();
            //    return new CsRedisCache(serializer, obj.GetService<ILoggerFactory>());
            //});

            //var csredis = new CSRedis.CSRedisClient(redisConfiguration.ToString());

            //////初始化 RedisHelper
            //RedisHelper.Initialization(csredis);
            #endregion

            services.AddRepositories();
            
            // 显式注册ProductService
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IInvitationService, InvitationService>();
            services.AddScoped<IRebateService, RebateService>();

            //防止汉字被自动编码
            services.Configure<WebEncoderOptions>(options =>
            {
                options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
            });
            //请求参数长度过大
            services.Configure<FormOptions>(options =>
            {

                options.ValueLengthLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = int.MaxValue;
                options.MultipartHeadersLengthLimit = int.MaxValue;

            });
            //记录错误
            services.AddMvc(options =>
            {
                options.Filters.Add<HttpGlobalExceptionFilter>();
            }).AddNewtonsoftJson(options =>
            {
                // Use the default property (Pascal) casing
                //options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                //options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            services.Configure<ImageConfig>(Configuration.GetSection("DataCenter:ImageConfig"));
            services.Configure<ApiConfig>(Configuration.GetSection("Admin:ApiConfig"));
           
            services.AddHttpClient();
            services.AddHttpClient<CustomHttpClientFactory>().ConfigurePrimaryHttpMessageHandler(c=>new CustomHttpMessageHandler());
            services.AddSingleton<IRedisCache>(obj =>
            {
                var redisConfiguration = Configuration.GetSection("Redis").Get<RedisConfiguration>();
                var serializer = new MsgPackSerializer();
                var connection = new PooledConnectionMultiplexer(redisConfiguration.ConfigurationOptions);
                return new RedisCache(obj.GetRequiredService<ILogger<RedisCache>>(), connection, redisConfiguration, serializer);
            });
          
              
            //razor 热重载
            services.AddRazorPages().AddRazorRuntimeCompilation();
            LogExtension.LogWarn($"启动成功，当前环境{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider svp, ILoggerFactory loggerFactory)
        {
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
                await next();
            });

            if (env.IsDevelopment())
            {

                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
                // app.UseHttpsRedirection();
            }
            loggerFactory.AddSerilog();
            //其他错误页面处理
            app.UseStatusCodePagesWithReExecute("/StatusCode/{0}");
            //启用Session
            app.UseSession();
            app.UseRouting();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseStaticHttpContext();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                      name: "areas",
                      pattern: "{area:exists}/{controller=Index}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            //使用环境变量
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseMiddlewareExtension(new ResultExceptionHandler());

        }
    }
}
