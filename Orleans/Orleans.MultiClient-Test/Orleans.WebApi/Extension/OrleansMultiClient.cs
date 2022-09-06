using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Common;
using Orleans.Grains;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Orleans.WebApi.Extension
{
    public static class OrleansMultiClient
    {
        public static IServiceCollection AddExtOrleansMultiClient(this IServiceCollection services, IConfiguration configuration)
        {
            var silo = configuration.GetSection("host").Get<SiloExOptions>();
            var siloCopy = configuration.GetSection("hostCopy").Get<SiloExOptions>();
            var siloCopyCopy = configuration.GetSection("hostCopyCopy").Get<SiloExOptions>();
            services.AddOrleansMultiClient(build =>
            {
                build.AddClient(opt =>
                {
                    opt.ServiceId = silo.ServiceId;
                    opt.ClusterId = silo.ClusterId;
                    opt.SetServiceAssembly(typeof(IHelloA).Assembly);
                    opt
                    .Configure = (b =>
                    {
                        b.UseStaticClustering(new IPEndPoint(IPAddress.Parse(silo.IpAddress), silo.GatewayPort)
                            , new IPEndPoint(IPAddress.Parse(siloCopy.IpAddress), siloCopy.GatewayPort),
                           new IPEndPoint(IPAddress.Parse(siloCopyCopy.IpAddress), siloCopyCopy.GatewayPort)
                           );
                    });
                });
            });
            return services;
        }
    }
}
