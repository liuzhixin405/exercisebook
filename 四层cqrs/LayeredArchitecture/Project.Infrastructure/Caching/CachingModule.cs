using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.Caching
{
    public class CachingModule:Module
    {
        private readonly Dictionary<string, TimeSpan> _expirationConfiguration;
        public CachingModule(Dictionary<string, TimeSpan> expirationConfiguration)
        {
            _expirationConfiguration = expirationConfiguration;
        }
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MemoryCacheStore>().As<ICacheStore>().WithParameter("expirationConfiguration",_expirationConfiguration)
                .SingleInstance();
        }
    }
}
