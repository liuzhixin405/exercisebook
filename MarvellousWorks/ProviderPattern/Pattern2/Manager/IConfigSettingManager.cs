using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProviderPattern
{
    /// <summary>
    /// 读取配置接口
    /// </summary>
    public interface IConfigSettingManager
    {
        /// <summary>
        /// 通过名称获取配置项
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        ConfigSetting GetSettingDefinition(string name);
        /// <summary>
        /// 获取全部配置项
        /// </summary>
        /// <returns></returns>
        IEnumerable<ConfigSetting> GetAllConfiSettings();
    }
}
