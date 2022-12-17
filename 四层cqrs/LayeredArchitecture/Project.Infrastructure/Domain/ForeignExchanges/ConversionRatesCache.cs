using Project.Domain.ForeignExchange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.Domain.ForeignExchanges
{
    public class ConversionRatesCache
    {
        public List<ConversionRate> Rates { get; }
        public ConversionRatesCache(List<ConversionRate> conversionRates)
        {
            Rates = conversionRates;
        }
    }
}
