
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;

namespace WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHostedService<ConsumerService>();
            builder.Services.AddHostedService<DeadLetterExchangeConsuerService>();
            builder.Services.AddHostedService<DelayExchangeConsumerService>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.MapGet("/normal/{message}", ([FromRoute] string message) =>
            {
                ConnectionFactory factory = new ConnectionFactory();
                factory.HostName = "localhost";
                factory.Port = 5672;
                using (IConnection connection = factory.CreateConnection())
                {
                    using (IModel channel = connection.CreateModel())
                    {
                        var queueName = "rbTest202301";
                        channel.QueueDeclare(queueName, true, false, false, null);

                        {
                            string sendMessage = string.Format("Message_{0}", message);
                            byte[] buffer = Encoding.UTF8.GetBytes(sendMessage);
                            IBasicProperties basicProperties = channel.CreateBasicProperties();
                            basicProperties.DeliveryMode = 2; //持久化  1=非持久化
                            channel.BasicPublish("", queueName, basicProperties, buffer);
                            Console.WriteLine("消息发送成功：" + sendMessage);
                        }
                    }
                }
            });

            app.MapGet("/deadletterexchange/{message}",([FromRoute] string message) =>{
                DeadLetterExchange.Send(message);
            });

            app.MapGet("/delayexchange/{message}", ([FromRoute] string message) => {
                DelayExchange.SendMessage(message);
            });
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}