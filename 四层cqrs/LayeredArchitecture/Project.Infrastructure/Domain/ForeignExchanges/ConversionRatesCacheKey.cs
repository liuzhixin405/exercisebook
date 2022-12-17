using Project.Infrastructure.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.Domain.ForeignExchanges
{
    public class ConversionRatesCacheKey : ICacheKey<ConversionRatesCache>
    {
        public string CacheKey => "ConversionRatesCache";
    }
}
