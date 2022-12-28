using System;
using System.Collections.Generic;

namespace CodeMan.Seckill.Base.RabbitMq
{
    public class QueueInfo
    {
        /// <summary>
        ///  队列名称
        /// </summary>
        public string Queue { get; set; }
        /// <summary>
        ///  路由名称
        /// </summary>
        public string RoutingKey { get; set; }
        /// <summary>
        ///  交换机类型
        /// </summary>
        public string ExchangeType { get; set; }
        /// <summary>
        /// 交换机名称
        /// </summary>
        public string Exchange { get; set; }

        public IDictionary<string, object> props { get; set; } = null;
        /// <summary>
        ///  接受消息委托
        /// </summary>
        public Action<RabbitMessageEntity> OnReceived { get; set; }
    }
}