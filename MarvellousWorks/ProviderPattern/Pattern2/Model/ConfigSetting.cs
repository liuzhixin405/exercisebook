using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProviderPattern
{
    /// <summary>
    /// 配置项
    /// </summary>
    public class ConfigSetting
    {
        public string Name { get; set; }
        public string DefaultValue { get; set; }
        public ConfigSetting(string name ,string defaultValue)
        {
            this.Name = name;
            this.DefaultValue = defaultValue;
        }
    }
}
