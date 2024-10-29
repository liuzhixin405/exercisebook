using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repository.Service.Consumers;
using Repository.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Service.Extensions
{
    public static class MassTransitExtension
    {
        public static void AddInternalMassTransit(this IServiceCollection services, IConfiguration configuration, ServiceType serviceType, string rabbitMqHost= "rabbitmq://localhost:5672")
        {
            services.AddMassTransit(x =>
            {
                switch (serviceType)
                {
                    case ServiceType.Product:
                        x.AddConsumer<CreateProductConsumer>();
                        break;
                    case ServiceType.Order:
                        x.AddConsumer<CreateOrderConsumer>();
                        break;

                    default:
                        throw new ArgumentException("Invalid service type");
                }
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(rabbitMqHost, h =>
                    {
                        //h.Username(configuration["RabbitMq:Username"]);
                        //h.Password(configuration["RabbitMq:Password"]);
                        h.Username("admin");
                        h.Password("admin");
                    });
                    cfg.ConfigureEndpoints(context); // 确保自动配置端点
                });
              
            });
        }
    }
}
