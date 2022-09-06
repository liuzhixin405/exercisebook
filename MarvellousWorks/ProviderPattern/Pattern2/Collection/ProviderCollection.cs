using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProviderPattern.Collection
{
    /// <summary>
    /// 集合提供者
    /// </summary>
    public class ProviderCollection
    {
        public IList<Type> Providers { get; private set; }
        public ProviderCollection()
        {
            this.Providers = new List<Type>();
        }
        public void AddProvider<T>()
        {
            if (typeof(SettingProvider).IsAssignableFrom(typeof(T)))
            {
                this.Providers.Add(typeof(T));
            }
        }
    }
}
