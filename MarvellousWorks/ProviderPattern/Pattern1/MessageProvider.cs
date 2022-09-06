using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProviderPattern
{
    public abstract class MessageProvider:ProviderBase
    {
        public abstract bool Insert(MessageModel model);
        public abstract List<MessageModel> Get();
    }

    public class SqlMessageProvider : MessageProvider
    {
        private string _connectionString="str";
        public override List<MessageModel> Get()
        {
            List<MessageModel> list = new List<MessageModel>();
            list.Add(new MessageModel($"Sql链接方式,链接字符串是{this._connectionString}", DateTime.Now));
            return list;
        }

        public override bool Insert(MessageModel model)
        {
            //省略一千行
            return true;
        }
        public override void Initialize(string name, NameValueCollection config)
        {
            if (string.IsNullOrWhiteSpace(name))
                name = "MessageProvier";
            if (null == config)
                throw new ArgumentNullException("config");
            if (string.IsNullOrWhiteSpace(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "SqlServer操作Message");
            }
            base.Initialize(name, config);

            string temp = config["connectionStringName"];
            if(temp==null|| temp.Length < 1)
            {
                throw new ProviderException("connectionString[temp]属性缺少或为空");
            }
            config.Remove("connectionStringName");
        }
    }
    public class XmlMessageProvider:MessageProvider
    {
        private string _connectionString;

        /// <summary>
        /// 插入Message
        /// </summary>
        /// <param name="mm">Message实体对象</param>
        /// <returns></returns>
        public override bool Insert(MessageModel mm)
        {
            // 代码略
            return true;
        }

        /// <summary>
        /// 获取Message
        /// </summary>
        /// <returns></returns>
        public override List<MessageModel> Get()
        {
            List<MessageModel> l = new List<MessageModel>();
            l.Add(new MessageModel("XML方式，连接字符串是" + this._connectionString, DateTime.Now));

            return l;
        }

        /// <summary>
        /// 初始化提供程序。
        /// </summary>
        /// <param name="name">该提供程序的友好名称。</param>
        /// <param name="config">名称/值对的集合，表示在配置中为该提供程序指定的、提供程序特定的属性。</param>
        public override void Initialize(string name, NameValueCollection config)
        {
            if (string.IsNullOrEmpty(name))
                name = "MessageProvider";

            if (null == config)
                throw new ArgumentException("config参数不能为null");

            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "XML操作Message");
            }

            base.Initialize(name, config);

            string temp = config["connectionStringName"];
            if (temp == null || temp.Length < 1)
                throw new ProviderException("connectionStringName属性缺少或为空");

            _connectionString = ConfigurationManager.ConnectionStrings[temp].ConnectionString;
            if (_connectionString == null || _connectionString.Length < 1)
            {
                throw new ProviderException("没找到'" + temp + "'所指的连接字符串，或所指连接字符串为空");
            }

            config.Remove("connectionStringName");
        }
    }
}
