using System;
using CodeMan.Seckill.Base.RabbitMq.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CodeMan.Seckill.Consumer.Email
{
    class Program
    {
        static void Main(string[] args)
        {
            var configRabbit = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .GetSection("RabbitMQ");

            var configEmail = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build()
                .GetSection("EmailKit");

            var host = new HostBuilder()
                .ConfigureServices(collection => collection
                    .AddSingleton(new RabbitConnection(configRabbit.Get<RabbitOption>()))
                    .AddSingleton(configEmail.Get<EmailOption>())
                    .AddSingleton<IHostedService, ProcessEmail>()
                    .AddScoped<EmailMessageSend>())
                .Build();
            host.Run();
        }
    }
}
