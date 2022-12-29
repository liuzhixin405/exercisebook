using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cat.Seckill.Base.RabbitMq.Config
{
    public class RabbitConnection
    {
        private readonly RabbitOption _config;
        private IConnection _connection = null;
        private IModel _model;
        public RabbitConnection(RabbitOption config)
        {
            _config = config;
        }

        public IConnection GetConnection()
        {
            if (_connection == null)
            {
                ConnectionFactory factory = new ConnectionFactory();
                factory.HostName = _config.Hostname;
                factory.Port = _config.Port;
                factory.UserName = _config.Username;
                factory.Password = _config.Password;
                factory.VirtualHost = _config.VirtualHost;
                _connection = factory.CreateConnection();
            }
            return _connection;
        }
        public IModel GetModel()
        {
            _connection = GetConnection();
            _model = _connection.CreateModel();
            return _model;
        }
    }
}
