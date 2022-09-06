using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProviderPattern
{
    public class CodeSettingProvider : SettingProvider
    {
        public override IEnumerable<ConfigSetting> GetSettingDefinitions()
        {
            return new List<ConfigSetting>
            {
                new ConfigSetting("Author.UserName","fan"),
                new ConfigSetting("Author.Age","18"),
                new ConfigSetting("Author.QQ","6768595491")
            };
        }
    }
}
