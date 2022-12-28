using System.Collections.Generic;
using System.Text;
using CodeMan.Seckill.Base.RabbitMq.Config;
using RabbitMQ.Client;

namespace CodeMan.Seckill.Base.RabbitMq
{
    public class RabbitProducer : IRabbitProducer
    {
        private readonly RabbitConnection _connection;

        public RabbitProducer(RabbitConnection connection)
        {
            _connection = connection;
        }
        public void Publish(string exchange, string routingKey, IDictionary<string, object> prop, string content)
        {
            var channel = _connection.GetModel();
            var props = channel.CreateBasicProperties();
            if (prop.Count > 0)
            {
                var delay = prop["x-delay"];
                props.Expiration = delay.ToString();
            }
            channel.BasicPublish(exchange, routingKey, false, props, Encoding.UTF8.GetBytes(content));
        }
    }
}