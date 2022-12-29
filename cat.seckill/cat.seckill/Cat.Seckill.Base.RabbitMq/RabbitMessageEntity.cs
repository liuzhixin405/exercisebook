using System;
using RabbitMQ.Client.Events;

namespace Cat.Seckill.Base.RabbitMq
{
    public class RabbitMessageEntity
    {
        public EventingBasicConsumer Consumer { get; set; }
        public BasicDeliverEventArgs BasicDeliver { get; set; }
        public int Code { get; set; }
        public string Content { get; set; }
        public bool Error { get; set; }
        public string ErrorMessage { get; set; }
        public Exception Exception { get; set; }
    }
}