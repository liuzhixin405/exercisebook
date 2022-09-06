using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ProviderPattern
{
    public class Message
    {
        private static bool m_isInitialized = false;
        private static MessageProviderCollection _providers = null;
        private static MessageProvider _provider = null;

        /// <summary>
        /// 静态构造函数，初始化
        /// </summary>
        static Message()
        {
            Initialize();
        }

        /// <summary>
        /// 插入信息
        /// </summary>
        /// <param name="mm">Message实体对象</param>
        /// <returns></returns>
        public static bool Insert(MessageModel mm)
        {
            return _provider.Insert(mm);
        }

        /// <summary>
        /// 获取信息
        /// </summary>
        /// <returns></returns>
        public static List<MessageModel> Get()
        {
            return _provider.Get();
        }
        private static void Initialize()
        {
            try
            {
                MessageProviderConfigurationSection messageConfig = null;

                if (!m_isInitialized)
                {

                    // 找到配置文件中“MessageProvider”节点
                    messageConfig = (MessageProviderConfigurationSection)ConfigurationManager.GetSection("MessageProvider");

                    if (messageConfig == null)
                        throw new ConfigurationErrorsException("在配置文件中没找到“MessageProvider”节点");

                    _providers = new MessageProviderCollection();

                    // 使用System.Web.Configuration.ProvidersHelper类调用每个Provider的Initialize()方法
                   // System.Web.Configuration.ProvidersHelper.InstantiateProviders(messageConfig.Providers, _providers, typeof(MessageProvider));

                    // 所用的Provider为配置中默认的Provider
                    _provider = (_providers[messageConfig.DefaultProvider]) as MessageProvider;

                    m_isInitialized = true;

                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                throw new Exception(msg);
            }
        }

        private static MessageProvider Provider
        {
            get
            {
                return _provider;
            }
        }

        private static MessageProviderCollection Providers
        {
            get
            {
                return _providers;
            }
        }
    }
}
