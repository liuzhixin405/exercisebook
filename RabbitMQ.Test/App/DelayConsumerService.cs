using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;

namespace App
{
    public class DelayConsumerService:BackgroundService
    {
        private readonly IModel channel;
        private readonly IConnection connection;
        public DelayConsumerService()
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "localhost";
            factory.Port = 5672;
            factory.UserName = "admin";
            factory.Password = "admin";
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            var queueName = "queue.dlx";
            channel.ExchangeDeclare("exchange.dlx", ExchangeType.Direct, true);
            //channel.QueueDeclare("queue.dlx", true, false, false, null);

            channel.QueueDeclare(queueName, true, false, false, null);

            channel.QueueBind(queueName, "exchange.dlx", "routingkey");  //可能是新版问题吧，不绑定routingkey消费不了。
            //输入1，那如果接收一个消息，但是没有应答，则客户端不会收到下一个消息
            channel.BasicQos(0, 1, false);
            //在队列上定义一个消费者
            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume("queue.dlx", false, consumer);
            consumer.Received += (ch, ea) =>
            {
                byte[] bytes = ea.Body.ToArray();
                string str = Encoding.UTF8.GetString(bytes);
                Console.WriteLine($"{DateTime.Now}超时未处理的消息: {str.ToString()}");
                //回复确认
                {
                    channel.BasicAck(ea.DeliveryTag, false);
                }
            };
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("delay Rabbitmq消费端开始工作!");
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
