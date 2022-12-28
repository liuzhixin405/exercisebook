using System;
using CodeMan.Seckill.Base.EntityFrameworkCore;
using CodeMan.Seckill.Base.EntityFrameworkCore.Repository;
using CodeMan.Seckill.Base.RabbitMq;
using CodeMan.Seckill.Base.RabbitMq.Config;
using CodeMan.Seckill.Base.Redis;
using CodeMan.Seckill.Service.Repository;
using CodeMan.Seckill.Service.service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CodeMan.Seckill.Consumer.Order
{
    class Program
    {
        static void Main(string[] args)
        {
            var configRabbit = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .GetSection("RabbitMQ");

            var configMysql = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .GetConnectionString("SeckillTestDB");

            var configRedis = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .GetSection("Redis:Default");


            var host = new HostBuilder()
                .ConfigureServices(collection => collection
                    .AddSingleton(new RabbitConnection(configRabbit.Get<RabbitOption>()))
                    .AddSingleton(new RedisHelper(configRedis.Get<RedisOption>()))
                    .AddSingleton<IHostedService, ProcessTest>()
                    .AddSingleton<IHostedService, ProcessOrder>()
                    .AddSingleton<IRabbitProducer, RabbitProducer>()
                    .AddScoped<ISeckillService, SeckillService>()
                    .AddScoped<IGoodsService, GoodsService>()
                    .AddScoped<IRepositoryWrapper, RepositoryWrapper>()
                    .AddScoped<IOrderService, OrderService>()
                    .AddDbContext<SeckillDbContext>(builder =>
                        builder.UseMySql(configMysql, ServerVersion.Parse("5.7"))))
                .Build();
            host.Run();
        }
    }
}
