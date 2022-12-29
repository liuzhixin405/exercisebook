using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cat.Seckill.Base.RabbitMq.Config
{
    public class RabbitOption
    {
        /// <summary>
        /// 消息队列的主机地址
        /// </summary>
        public string Hostname { get; set; }
        /// <summary>
        /// 消息队列的端口号
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 消息队列的用户名
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// 消息队列的密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 消息队列的虚拟主机
        /// </summary>
        public string VirtualHost { get; set; }
    }
}
