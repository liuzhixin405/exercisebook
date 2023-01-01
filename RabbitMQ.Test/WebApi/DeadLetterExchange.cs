using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace WebApi
{
    public class DeadLetterExchange
    {
        public static string dlxExchange = "dlx.exchange";
        public static string dlxQueueName = "dlx.queue";
        static string exchange = "direct-exchange";
        static string queueName = "queue_Testdlx";
        static string dlxExchangeKey = "x-dead-letter-exchange";
        static string dlxQueueKey = "x-dead-letter-rounting-key";
        public static void Send(string message)
        {
            using (var connection = new ConnectionFactory() { HostName = "localhost", Port = 5672 }.CreateConnection())
            {
                using(var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(dlxExchange, ExchangeType.Direct, true, false); //创建sixin交换机
                    channel.QueueDeclare(dlxQueueName, true, false, false); // 创建sixin队列
                    channel.QueueBind(dlxQueueName, dlxExchange, dlxQueueName); //绑定sixin队列sixin交换机

                    channel.ExchangeDeclare(exchange, ExchangeType.Direct, true, false); //创建交换机
                    channel.QueueDeclare(queueName, true, false, false,new Dictionary<string, object>
                    {
                        { dlxExchangeKey,dlxExchange },
                        {dlxQueueKey,dlxQueueName }
                    }); // 创建队列
                    channel.QueueBind(queueName, exchange, queueName);
                    

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent= true;//持久化
                    channel.BasicPublish(exchange,queueName,properties,Encoding.UTF8.GetBytes(message));
                    Console.WriteLine($"向队列:{queueName}发送消息:{message}");
                }
            }
        }
        
        public static void Consumer()
        {
            var connection = new ConnectionFactory() { HostName = "localhost", Port = 5672 }.CreateConnection();
            var channel = connection.CreateModel();
            channel.ExchangeDeclare(dlxExchange, ExchangeType.Direct, true, false); //创建sixin交换机
            channel.QueueDeclare(dlxQueueName, true, false, false); // 创建sixin队列
            channel.QueueBind(dlxQueueName, dlxExchange, dlxQueueName); //绑定sixin队列sixin交换机

            channel.ExchangeDeclare(exchange, ExchangeType.Direct, true, false); //创建交换机
            channel.QueueDeclare(queueName, true, false, false, new Dictionary<string, object>
                    {
                        { dlxExchangeKey,dlxExchange },
                        {dlxQueueKey,dlxQueueName }
                    }); // 创建队列
            channel.QueueBind(queueName, exchange, queueName);

            var consumer = new EventingBasicConsumer(channel);
            channel.BasicQos(0, 1, false);
            consumer.Received += (model, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                Console.WriteLine($"队列{queueName}消费消息:{message},不做ack确认");
                channel.BasicNack(ea.DeliveryTag, false, requeue: false);
            };
            channel.BasicConsume(queueName, autoAck: false, consumer);
        }
    }
}
