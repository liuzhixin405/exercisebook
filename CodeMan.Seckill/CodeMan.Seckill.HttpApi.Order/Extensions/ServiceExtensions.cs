using CodeMan.Seckill.Base.EntityFrameworkCore;
using CodeMan.Seckill.Base.EntityFrameworkCore.Repository;
using CodeMan.Seckill.Base.RabbitMq.Config;
using CodeMan.Seckill.Base.Redis;
using CodeMan.Seckill.Service.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeMan.Seckill.HttpApi.Order.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AnyPolicy",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });
        }

        public static void ConfigureMySqlContext(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config.GetConnectionString("SeckillTestDB");
            services.AddDbContext<SeckillDbContext>(
                builder => builder.UseMySql(connectionString, ServerVersion.Parse("5.7")));
        }

        public static void ConfigureRedisContext(this IServiceCollection services, IConfiguration config)
        {
            var section = config.GetSection("Redis:Default");
            services.AddSingleton(new RedisHelper(section.Get<RedisOption>()));
        }

        public static void ConfigureRabbitContext(this IServiceCollection services, IConfiguration config)
        {
            var section = config.GetSection("RabbitMQ");
            services.AddSingleton(
                new RabbitConnection(section.Get<RabbitOption>()));
        }

        public static void ConfigureRepositoryWrapper(this IServiceCollection services)
        {
            services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
        }
    }
}