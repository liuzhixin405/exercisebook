using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeMan.Seckill.Base.EntityFrameworkCore.Extensions
{
    public static class ServiceExtensions
    {
        // public static void ConfigureCors(this IServiceCollection services)
        // {
        //     services.AddCors(options =>
        //     {
        //         options.AddPolicy("AnyPolicy",
        //             builder => builder.AllowAnyOrigin()
        //                 .AllowAnyMethod()
        //                 .AllowAnyHeader());
        //     });
        // }

        public static void ConfigureMySqlContext(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config.GetConnectionString("SeckillTestDB");
            services.AddDbContext<SeckillDbContext>(
                builder => builder.UseMySql(connectionString, ServerVersion.Parse("5.7")));
        }

        // public static void ConfigureRedisContext(this IServiceCollection services, IConfiguration config)
        // {
        //     var section = config.GetSection("Redis:Default");
        //     var connection = section.GetSection("Connection").Value;
        //     var instanceName = section.GetSection("InstanceName").Value;
        //     var defaultDB = int.Parse(section.GetSection("DefaultDB").Value ?? "0");
        //     services.AddSingleton(new RedisHelper(connection, instanceName, defaultDB));
        // }

        // public static void ConfigureRabbitContext(this IServiceCollection services, IConfiguration config)
        // {
        //     var section = config.GetSection("RabbitMQ");
        //     var hostname = section.GetSection("Hostname").Value;
        //     var port = int.Parse(section.GetSection("Port").Value);
        //     var username = section.GetSection("Username").Value;
        //     var password = section.GetSection("Password").Value;
        //     var virtualHost = section.GetSection("VirtualHost").Value;
        //     services.AddSingleton(
        //         new RabbitConnection(new RabbitConfigEntity(hostname, port, username, password, virtualHost)));
        // }

        // public static void ConfigureRepositoryWrapper(this IServiceCollection services)
        // {
        //     services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
        // }
    }
}