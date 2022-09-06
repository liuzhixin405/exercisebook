using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProviderPattern.Collection;

namespace ProviderPattern
{
    /// <summary>
    /// 读取配置实现
    /// </summary>
    public class ConfigSettingManager : IConfigSettingManager
    {
        private readonly ProviderCollection _providerCollection = null;
        private readonly IDictionary<string, ConfigSetting> _confiSettings = null;
        public ConfigSettingManager(ProviderCollection providerCollection)
        {
            this._providerCollection = providerCollection;
            this._confiSettings = new Dictionary<string, ConfigSetting>();
        }
        public void Initialize()
        {
            foreach (var providerType in this._providerCollection.Providers)
            {
                if(Activator.CreateInstance(providerType) is SettingProvider provider)
                {
                    foreach (var configSetting in provider.GetSettingDefinitions())
                    {
                        if(!this._confiSettings.ContainsKey(configSetting.Name))
                        this._confiSettings.Add(configSetting.Name, configSetting);
                    }
                }
            }
        }

        public IEnumerable<ConfigSetting> GetAllConfiSettings()
        {
            return _confiSettings.Values;
        }

        public ConfigSetting GetSettingDefinition(string name)
        {
            return this._confiSettings[name];
        }
    }
}
