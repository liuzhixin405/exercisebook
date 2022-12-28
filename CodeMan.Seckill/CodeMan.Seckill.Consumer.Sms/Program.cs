using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using System;
using CodeMan.Seckill.Base.RabbitMq.Config;
using Microsoft.Extensions.DependencyInjection;

namespace CodeMan.Seckill.Consumer.Sms
{
    class Program
    {
        static void Main(string[] args)
        {
            var configRabbit = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .GetSection("RabbitMQ");

            var host = new HostBuilder()
                .ConfigureServices(collection => collection
                    .AddSingleton(new RabbitConnection(configRabbit.Get<RabbitOption>()))
                    .AddSingleton<IHostedService, ProcessSms>()
                    .AddScoped<SmsMessage>())
                .Build();
            host.Run();
        }
    }
}
