using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProviderPattern
{
    /// <summary>
    /// Xml配置提供者
    /// </summary>
    public class XmlSettingProvider : SettingProvider
    {
        public override IEnumerable<ConfigSetting> GetSettingDefinitions()
        {
            return new List<ConfigSetting> { new ConfigSetting("xml001", "vvv") };
        }
    }
}
