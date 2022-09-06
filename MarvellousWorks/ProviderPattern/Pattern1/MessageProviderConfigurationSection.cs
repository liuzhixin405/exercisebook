using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProviderPattern
{
    public class MessageProviderConfigurationSection:ConfigurationSection
    {
        private readonly ConfigurationProperty _defaultProvider;
        private readonly ConfigurationProperty _providers;
        private ConfigurationPropertyCollection _properties;

        public MessageProviderConfigurationSection()
        {
            _defaultProvider = new ConfigurationProperty("defaultProvider", typeof(string), null);
            _providers = new ConfigurationProperty("providers", typeof(ProviderSettingsCollection), null);
            _properties = new ConfigurationPropertyCollection();

            _properties.Add(_providers);
            _properties.Add(_defaultProvider);
        }

        /// <summary>
        /// Message的默认的Provider
        /// </summary>
        [ConfigurationProperty("defaultProvider")]
        public string DefaultProvider
        {
            get { return (string)base[_defaultProvider]; }
            set { base[_defaultProvider] = value; }
        }

        /// <summary>
        /// Message的所有的Provider集合
        /// </summary>
        [ConfigurationProperty("providers", DefaultValue = "SqlMessageProvider")]
        [StringValidator(MinLength = 1)]
        public ProviderSettingsCollection Providers
        {
            get { return (ProviderSettingsCollection)base[_providers]; }
        }

        /// <summary>
        /// Message的Provider的属性集合
        /// </summary>
        protected override ConfigurationPropertyCollection Properties
        {
            get { return _properties; }
        }
    }
}
