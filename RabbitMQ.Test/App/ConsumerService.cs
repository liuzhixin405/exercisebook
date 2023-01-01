using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace App
{
    public class ConsumerService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("normal Rabbitmq消费端开始工作!");
            while (!stoppingToken.IsCancellationRequested)
            {
                ConnectionFactory factory = new ConnectionFactory();
                factory.HostName = "localhost";
                factory.Port = 5672;

                IConnection connection = factory.CreateConnection();
                {
                    IModel channel = connection.CreateModel();
                    {
                        var queueName = "rbTest2023010";
                        channel.ExchangeDeclare("exchange.normal", ExchangeType.Fanout, true);
                        channel.QueueDeclare(queueName, true, false, false, new Dictionary<string, object>
                        {
                            { "x-message-ttl" ,10000},
                            {"x-dead-letter-exchange","exchange.dlx" },
                            {"x-dead-letter-routing-key","routingkey" }
                        });

                        channel.QueueBind(queueName, "exchange.normal", "");
                        //输入1，那如果接收一个消息，但是没有应答，则客户端不会收到下一个消息
                        channel.BasicQos(0, 1, false);
                        //在队列上定义一个消费者
                        var consumer = new EventingBasicConsumer(channel);
                        channel.BasicConsume(queueName, false, consumer);
                        consumer.Received += (ch, ea) =>
                        {
                            byte[] bytes = ea.Body.ToArray();
                            string str = Encoding.UTF8.GetString(bytes);
                            Console.WriteLine($"{DateTime.Now}来自死信队列获取的消息: {str.ToString()}");
                            //回复确认
                            if (str.Contains("跳过")) //假设超时不处理，留给后面deadconsumerservice处理
                            {
                                Console.WriteLine($"{DateTime.Now}来自死信队列获取的消息: {str.ToString()},该消息被拒绝");
                                channel.BasicNack(ea.DeliveryTag, false,false);
                            }
                            else  //正常消息处理
                            {
                                Console.WriteLine($"{DateTime.Now}来自死信队列获取的消息: {str.ToString()}，该消息被接受");
                                channel.BasicAck(ea.DeliveryTag, false);
                            }
                        };

                    }
                }
                await Task.Delay(5000);
            }
        }
    }
}