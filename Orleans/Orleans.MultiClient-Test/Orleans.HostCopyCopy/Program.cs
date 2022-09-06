using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans.Common;
using Orleans.Configuration;
using Orleans.Grains;
using Orleans.Hosting;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Orleans.HostCopyCopy
{
    public class Program
    {
        public static int Main(string[] args)
        {
            return RunMainAsync().Result;
        }

        private static async Task<int> RunMainAsync()
        {
            Console.Title = "HostCopyCopy";
            try
            {
                var host = await StartSilo();
                Console.WriteLine("Press Enter to terminate...");
                Console.ReadLine();

                await host.StopAsync();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 1;
            }
        }

        private static async Task<ISiloHost> StartSilo()
        {
            IConfiguration jsonConfiguration = new ConfigurationBuilder()
    .AddJsonFile("orleansconfig.json") // 添加 Json 文件配置 异常处理，
                                       .Build();
            var siloOptions = jsonConfiguration.GetSection("hostCopyCopy").Get<SiloExOptions>();
            var builder = new SiloHostBuilder()
                .UseLocalhostClustering(siloOptions.SiloPort, siloOptions.GatewayPort)
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = siloOptions.ClusterId;
                    options.ServiceId = siloOptions.ServiceId;
                })
                .Configure<EndpointOptions>(options =>
                {
                    // Port to use for Silo-to-Silo
                    options.SiloPort = siloOptions.SiloPort;
                    // Port to use for the gateway
                    options.GatewayPort = siloOptions.GatewayPort;
                    // IP Address to advertise in the cluster
                    options.AdvertisedIPAddress = IPAddress.Parse(siloOptions.IpAddress);
                    // The socket used for silo-to-silo will bind to this endpoint
                    options.GatewayListeningEndpoint = new IPEndPoint(IPAddress.Any, siloOptions.GatewayPort);
                    // The socket used by the gateway will bind to this endpoint
                    options.SiloListeningEndpoint = new IPEndPoint(IPAddress.Any, siloOptions.SiloPort);
                })
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(HelloGrain).Assembly).WithReferences())
                .ConfigureLogging(logging => logging.AddConsole());

            var host = builder.Build();
            await host.StartAsync();
            return host;
        }
    }
}
