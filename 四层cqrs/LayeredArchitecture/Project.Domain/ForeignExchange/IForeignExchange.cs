using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.ForeignExchange
{
    public interface IForeignExchange
    {
        List<ConversionRate> GetConversionRates();
    }
}
