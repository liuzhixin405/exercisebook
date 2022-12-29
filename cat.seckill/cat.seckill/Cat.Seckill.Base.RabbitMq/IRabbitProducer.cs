using RabbitMQ.Client;

using System.Collections.Generic;

namespace Cat.Seckill.Base.RabbitMq
{
    public interface IRabbitProducer
    {
        public void Publish(string exchange, string routingKey, IDictionary<string, object> prop, string content);
    }
}