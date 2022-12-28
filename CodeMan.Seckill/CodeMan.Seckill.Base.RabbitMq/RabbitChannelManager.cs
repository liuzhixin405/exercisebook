using System.Collections.Generic;
using CodeMan.Seckill.Base.RabbitMq.Config;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CodeMan.Seckill.Base.RabbitMq
{
    public class RabbitChannelManager
    {
        public RabbitConnection Connection { get; set; }

        public RabbitChannelManager(RabbitConnection connection)
        {
            this.Connection = connection;
        }

        public RabbitChannelManager() { }

        public RabbitChannelConfig CreateReceiveChannel(string exchangeType, string exchange, string queue, string routeKey, IDictionary<string, object> arguments = null)
        {
            IModel model = this.CreateModel(exchangeType, exchange, queue, routeKey, arguments);
            //model.BasicQos(0, 1, false);
            EventingBasicConsumer consumer = this.CreateConsumer(model, queue);
            RabbitChannelConfig channel = new RabbitChannelConfig(exchangeType, exchange, queue, routeKey);
            consumer.Received += channel.Receive;
            return channel;
        }

        /// <summary>
        /// 创建一个通道 包含交换机/队列/路由，并建立绑定关系
        /// </summary>
        /// <param name="type">交换机类型:Topic、Direct、Fanout</param>
        /// <param name="exchange">交换机名称</param>
        /// <param name="queue">队列名称</param>
        /// <param name="routeKey">路由名称</param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public IModel CreateModel(string exchangeType, string exchange, string queue, string routeKey, IDictionary<string, object> arguments = null)
        {
            exchangeType = string.IsNullOrEmpty(exchangeType) ? "default" : exchangeType;
            IModel model = this.Connection.GetConnection().CreateModel();
            model.BasicQos(0, 1, false);
            model.QueueDeclare(queue, true, false, false, arguments);
            model.ExchangeDeclare(exchange, exchangeType);
            model.QueueBind(queue, exchange, routeKey);
            return model;
        }
        public EventingBasicConsumer CreateConsumer(IModel model, string queue)
        {
            EventingBasicConsumer consumer = new EventingBasicConsumer(model);
            model.BasicConsume(queue, false, consumer);
            return consumer;
        }
    }
}