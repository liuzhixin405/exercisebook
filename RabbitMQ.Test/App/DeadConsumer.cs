using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;

namespace App
{
    public class DeadConsumer:BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("normal Rabbitmq消费端开始工作!");
            while (!stoppingToken.IsCancellationRequested)
            {
                ConnectionFactory factory = new ConnectionFactory();
                factory.HostName = "localhost";
                factory.Port = 5672;

                #region 第一版
                //IConnection connection = factory.CreateConnection();
                //{
                //    IModel channel = connection.CreateModel();
                //    {
                //        var queueName = "rbTest2023010";
                //        channel.ExchangeDeclare("exchange.dlx", ExchangeType.Direct, true);
                //        channel.QueueDeclare("queue.dlx", true, false, false,null);
                //        channel.QueueBind("queue.dlx", "exchange.dlx", "routingkey");
                //        //输入1，那如果接收一个消息，但是没有应答，则客户端不会收到下一个消息
                //        channel.BasicQos(0, 1, false);
                //        //在队列上定义一个消费者
                //        var consumer = new EventingBasicConsumer(channel);
                //        channel.BasicConsume("queue.dlx", false, consumer);
                //        consumer.Received += (ch, ea) =>
                //        {
                //            byte[] bytes = ea.Body.ToArray();
                //            string str = Encoding.UTF8.GetString(bytes);
                //            Console.WriteLine($"{DateTime.Now}来自死信队列获取的消息: {str.ToString()}");
                //            //回复确认
                //            channel.BasicAck(ea.DeliveryTag, false);
                //        };

                //    }
                // } 
                #endregion

                IConnection connection = factory.CreateConnection();
                {
                    IModel channel = connection.CreateModel();
                    {
                        var queueName = "queue.dlx";
                        channel.ExchangeDeclare("exchange.dlx", ExchangeType.Direct, true);
                        channel.QueueDeclare("queue.dlx", true, false, false, null);
                        //channel.ExchangeDeclare("exchange.normal", ExchangeType.Fanout, true);
                        channel.QueueDeclare(queueName, true, false, false, null);

                        channel.QueueBind(queueName, "exchange.dlx", "");
                        //输入1，那如果接收一个消息，但是没有应答，则客户端不会收到下一个消息
                        channel.BasicQos(0, 1, false);
                        //在队列上定义一个消费者
                        var consumer = new EventingBasicConsumer(channel);
                        channel.BasicConsume("queue.dlx", false, consumer);
                        consumer.Received += (ch, ea) =>
                        {
                            byte[] bytes = ea.Body.ToArray();
                            string str = Encoding.UTF8.GetString(bytes);
                            Console.WriteLine($"{DateTime.Now}来自最后的处理: {str.ToString()}");
                            //回复确认
                            {
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
