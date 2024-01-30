using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace App
{
    public class ConsumerService : BackgroundService
    {
        private readonly IModel channel;
        private readonly IConnection connection;
        public ConsumerService()
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "localhost";
            factory.Port = 5672;
            factory.UserName = "admin";
            factory.Password = "admin";
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            //var queueName = "rbTest2023010";
            //channel.ExchangeDeclare("exchange.normal", ExchangeType.Fanout, true);
            //channel.QueueDeclare("queue.dlx", true, false, false, new Dictionary<string, object>
            //            {
            //                { "x-message-ttl" ,1000*60},
            //                {"x-dead-letter-exchange","exchange.dlx" },
            //                {"x-dead-letter-routing-key","routingkey" }
            //            });

            //channel.QueueBind("queue.normal", "exchange.normal", "");
            ////输入1，那如果接收一个消息，但是没有应答，则客户端不会收到下一个消息
            //channel.BasicQos(0, 1, false);
            ////在队列上定义一个消费者
            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume("queue.normal", false, consumer);
            //consumer.Received += (ch, ea) =>
            //{
            //    byte[] bytes = ea.Body.ToArray();
            //    string str = Encoding.UTF8.GetString(bytes);
            //    Console.WriteLine($"{DateTime.Now}来自死信队列获取的消息: {str.ToString()}");
            //    //回复确认
            //    if (str.Contains("跳过")) //假设超时不处理，留给后面deadconsumerservice处理
            //    {
            //        Console.WriteLine($"{DateTime.Now}来自死信队列获取的消息: {str.ToString()},该消息被拒绝");
            //        channel.BasicNack(ea.DeliveryTag, false, false);
            //    }
            //    else  //正常消息处理
            //    {
            //        Console.WriteLine($"{DateTime.Now}来自死信队列获取的消息: {str.ToString()}，该消息被接受");
            //        channel.BasicAck(ea.DeliveryTag, false);
            //    }
            //};

            consumer.Received += (obj, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                //打印消费的消息
                Console.WriteLine($"消费的消息为:{message} 消费时间为：{DateTime.Now}");
                channel.BasicAck(ea.DeliveryTag, false);
            };

        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("normal Rabbitmq消费端开始工作!");
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(5000);
            }
        }
        public override void Dispose()
        {
            // 在服务结束时关闭连接和通道
            channel.Close();
            connection.Close();
            base.Dispose();
        }
    }
}