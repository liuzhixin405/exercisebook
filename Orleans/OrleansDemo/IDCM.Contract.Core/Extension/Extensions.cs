using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans;
using Orleans.Statistics;
using System.Net;
using Microsoft.Extensions.Configuration;
using Orleans.Hosting;
using Orleans.Configuration;

namespace IDCM.Contract.Core.Extension
{
    public static partial class Extensions
    {
        public static IHostBuilder UseExtOrleans(this IHostBuilder builder, Action<ISiloBuilder> action)
        {
            SiloExOptions siloOptions = new SiloExOptions();
            builder.ConfigureServices((host, service) =>
            {
                siloOptions = host.Configuration.GetSection("silo").Get<SiloExOptions>();
                if (siloOptions == null)
                {
                    throw new Exception("Silo初始化异常：缺少配置信息");
                }
            })
            .UseOrleans((builder) =>
            {
                builder.UseLocalhostClustering(siloPort:siloOptions.SiloPort,gatewayPort:siloOptions.GatewayPort)
                        .Configure<ClusterOptions>(options =>
                        {
                            options.ClusterId = siloOptions.ClusterId;
                            options.ServiceId = siloOptions.ServiceId;             //获取或设置此服务的唯一标识符，该标识符应在部署和重新部署后仍然有效
                        })
                .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback)//配置Silo的端口                                                                                                                     
                .UseDashboard(options =>
                {
                    options.Username = siloOptions.Username;
                    options.Password = siloOptions.Password;
                    options.Host = siloOptions.Host;
                    options.Port = siloOptions.Port;
                    options.HostSelf = siloOptions.HostSelf;
                    options.CounterUpdateIntervalMs = siloOptions.CounterUpdateIntervalMs;
                })//注册Dashboard
                .UsePerfCounterEnvironmentStatistics()//添加主机CPU和内存监控
                .AddApplicationInsightsTelemetryConsumer("INSTRUMENTATION_KEY");
                ;
                action(builder);
            });
            return builder;

        }
    }

    public class SiloExOptions
    {
        public string ClusterId { get; set; }
        public string ServiceId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool HostSelf { get; set; }
        public int CounterUpdateIntervalMs { get; set; }
        public int SiloPort { get; set; }
        public int GatewayPort { get; set; }    
    }
}
