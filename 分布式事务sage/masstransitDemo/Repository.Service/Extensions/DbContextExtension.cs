using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repository.Service.Enums;
using Repository.Service.Orders;
using Repository.Service.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Service.Extensions
{
    public static class DbContextExtension
    {
        public static IServiceCollection AddMysqlServices(this IServiceCollection services,ServiceType serviceType,IConfiguration configuration)
        {    
            switch (serviceType)
            {
                case ServiceType.Order:
                    var orderconnectionString = configuration["DbString:OrderDbConnection"] ?? throw new ArgumentException("DefaultConnection is not found in configuration");
                    services.AddDbContext<OrderDbContext>(options => options.UseMySql(orderconnectionString, ServerVersion.AutoDetect(orderconnectionString)));
                    break;
                case ServiceType.Product:
                    var productconnectionString = configuration["DbString:ProductDbConnection"] ?? throw new ArgumentException("ProductConnection is not found in configuration");
                    services.AddDbContext<ProductDbContext>(options => options.UseMySql(productconnectionString, ServerVersion.AutoDetect(productconnectionString)));
                    break;
                default:
                    break;
            }
            return services;
        }

        public static IServiceCollection AddInternalOrderServices(this IServiceCollection services)
        {
            services.AddScoped<OrderService>();
            return services;
        }
        public static IServiceCollection AddInternalProductServices(this IServiceCollection services)
        {
            services.AddScoped<ProductService>();
            return services;
        }
    }
}
