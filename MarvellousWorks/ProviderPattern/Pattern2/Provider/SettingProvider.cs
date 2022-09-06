using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProviderPattern
{
    /// <summary>
    /// 配置提供者,用来返回具体的配置项列表
    /// </summary>
    public abstract class SettingProvider
    {
        public abstract IEnumerable<ConfigSetting> GetSettingDefinitions();
    }
}
