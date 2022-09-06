using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProviderPattern
{
    public class MessageProviderCollection:ProviderCollection
    {
        public override void Add(ProviderBase provider)
        {
            if (provider == null) throw new ArgumentNullException("provider");
            if(!(provider is MessageProvider))
            {
                throw new ArgumentException("provider参数类型必须是MessageProvider");
            }
            base.Add(provider);
        }
    }
}
