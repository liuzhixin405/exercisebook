
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;

namespace App
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
            builder.Services.AddHostedService<DelayConsumerService>();
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
                factory.UserName = "admin";
                factory.Password = "admin";
                using (IConnection connection = factory.CreateConnection())
                {
                    using (IModel channel = connection.CreateModel())
                    {
                        var queueName = "rbTest2023010";
                      
                        //channel.ExchangeDeclare("exchange.dlx", ExchangeType.Direct, true);
                        //channel.QueueDeclare("queue.dlx", true, false, false, null);
                        channel.ExchangeDeclare("exchange.normal", ExchangeType.Fanout, true);
                        channel.QueueDeclare(queueName, true, false, false,
                            new Dictionary<string, object>
                        {
                            { "x-message-ttl" ,10000},
                            {"x-dead-letter-exchange","exchange.dlx" },
                            {"x-dead-letter-routing-key","routingkey" }
                        }
                            );
                       
                        channel.QueueBind(queueName, "exchange.normal", "");
                        {
                            string sendMessage = string.Format("Message_{0}", message);
                            byte[] buffer = Encoding.UTF8.GetBytes(sendMessage);
                            IBasicProperties basicProperties = channel.CreateBasicProperties();
                            basicProperties.DeliveryMode = 2; //�־û�  1=�ǳ־û�
                            channel.BasicPublish("exchange.normal", queueName, basicProperties, buffer);
                            Console.WriteLine($"{DateTime.Now}��Ϣ���ͳɹ���{sendMessage}" );
                        }
                    }
                }
            });
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}