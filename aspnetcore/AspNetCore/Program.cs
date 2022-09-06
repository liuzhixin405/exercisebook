using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using AspNetCore.Expressions;
using AspNetCore.Observer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HostFiltering;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AspNetCore
{
    public delegate void Do(string name);
    public class Program
    {
        public static void Show(string str)
        {
            Console.WriteLine(str);
        }
        private static void Do(string str, Action<string> act)
        {
            act(str);
        }
        private static IEnumerable<TSource> ExtensionsWhere<TSource>(IEnumerable<TSource> source,Func<TSource,bool> func)
        {
            List<TSource> list = new List<TSource>();
            foreach (var item in source)
            {
                if (func(item))
                {
                    list.Add(item);
                }
            }
            return list;
        }
        public static void Main(string[] args)
        {
            new PerformanceTesting().Invoke();
            new ExpressionTest().ClassCopy();
            new ExpressionTest().ShowWhere();
            new ExpressionTest().ShowPeople();
            new ExpressionTest().Show();
            new EventTest().DoSomeThing();
            Do("Hello world", Show);
            Action<string> action = (o) => { Console.WriteLine(o); };
            action("hello world");
            Action<IServiceCollection> collection = (o) => { Console.WriteLine(o.GetType().Name); };
            collection(new ServiceCollection());
            
            Test test = new Test();
            test.AddAction();
            test.Invoke();
            //注册
            ConcreteSubject cs = new ConcreteSubject();
            cs.Attach(new ConcreteObserver(cs, "X"));
            cs.Attach(new ConcreteObserver(cs, "Y"));
            cs.Attach(new ConcreteObserver(cs, "Z"));
            cs.SubjectState = "ABC";
            cs.Notify();
            CreateHostBuilder(args).Build().Run();
            //两个重要的类
            /*
             GenericWebHostService      //StartAsync
             HostBuilder    //CreateServiceProvider
             */
        }

        /// <summary>
        ///    Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()=>
        ///    Microsoft.Extensions.Hosting.GenericHostWebHostBuilderExtensions.ConfigureWebHost()=>
        ///    Microsoft.Extensions.Hosting.HostBuilder.ConfigureServices() => //添加Action<HostBuilderContext, IServiceCollection> configureServicesActions 委托
        ///    Microsoft.Extensions.Hosting.GenericHostBuilderExtensions.ConfigureWebHostDefaults()=>
        ///    Microsoft.AspNetCore.WebHost.ConfigureWebDefaults()   //添加kestrel
        ///    Microsoft.Extensions.Hosting.Build()
        ///    /*
        ///    public static IHostBuilder ConfigureWebHostDefaults(this IHostBuilder builder, Action<IWebHostBuilder> configure)
        //{
        //	return builder.ConfigureWebHost(delegate (IWebHostBuilder webHostBuilder)
        //	{
        //		WebHost.ConfigureWebDefaults(webHostBuilder);
        //		configure(webHostBuilder);
        //  });
        //}
        /// 
        /// 
        ///     */
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                 .ConfigureWebHostDefaults(webBuilder =>
                {
                    //webBuilder.UseKestrel(o => o.Listen(IPAddress.Loopback, 11111));
                    //webBuilder.ConfigureAppConfiguration((ctx, cb) =>
                    //{
                    //    if (ctx.HostingEnvironment.IsDevelopment())
                    //    {
                    //        StaticWebAssetsLoader.UseStaticWebAssets(ctx.HostingEnvironment, ctx.Configuration);
                    //    }
                    //});
                    //webBuilder.ConfigureServices((hostingContext, services) => {
                    //    services.PostConfigure<HostFilteringOptions>(options => { 
                    //    if(options.AllowedHosts==null&& options.AllowedHosts.Count == 0)
                    //        {
                    //            var hosts = hostingContext.Configuration["AllowedHosts"]?.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    //            options.AllowedHosts = (hosts?.Length > 0 ? hosts : new[] { "*" });
                    //        }
                    //    });
                    //    services.AddRouting();
                    //});
                    webBuilder.UseStartup<Startup>();
                });


    }
}
