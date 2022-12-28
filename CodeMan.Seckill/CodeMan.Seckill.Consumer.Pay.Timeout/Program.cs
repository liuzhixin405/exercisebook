using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using System;
using CodeMan.Seckill.Base.EntityFrameworkCore;
using CodeMan.Seckill.Base.EntityFrameworkCore.Repository;
using CodeMan.Seckill.Base.RabbitMq;
using CodeMan.Seckill.Base.RabbitMq.Config;
using CodeMan.Seckill.Service.Repository;
using CodeMan.Seckill.Service.service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CodeMan.Seckill.Consumer.Pay.Timeout
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

            var host = new HostBuilder()
                .ConfigureServices(collection => collection
                    .AddSingleton(new RabbitConnection(configRabbit.Get<RabbitOption>()))
                    .AddSingleton<IHostedService, ProcessPayTimeout>()
                    .AddScoped<IRepositoryWrapper, RepositoryWrapper>()
                    .AddScoped<IRabbitProducer, RabbitProducer>()
                    .AddScoped<IPayService, PayService>()
                    .AddDbContext<SeckillDbContext>(builder =>
                        builder.UseMySql(configMysql, ServerVersion.Parse("5.7"))))
                .Build();
            host.Run();
        }
    }
}
