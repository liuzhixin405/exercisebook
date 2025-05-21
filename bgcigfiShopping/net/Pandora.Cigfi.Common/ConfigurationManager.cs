using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandora.Cigfi.Common
{
    /// <summary>
    /// 配置单例管理类
    /// </summary>
    public interface IConfigurationManager
    {
        /// <summary>
        /// 获取配置
        /// </summary>
        T GetAppConfig<T>(string key, T defaultValue = default(T));
    }
    /// <summary>
    /// 配置单例管理类
    /// </summary>
    public class ConfigurationManager : IConfigurationManager
    {
        private readonly IConfiguration config;

        public ConfigurationManager(IConfiguration config)
            => this.config = config ?? throw new ArgumentNullException(nameof(config));

        /// <summary>
        /// 获取配置
        /// </summary>
        public T GetAppConfig<T>(string key, T defaultValue = default(T))
        {
            T setting = (T)Convert.ChangeType(config[key], typeof(T));
            var value = setting;
            if (setting == null)
                value = defaultValue;
            return value;
        }
    }
}
