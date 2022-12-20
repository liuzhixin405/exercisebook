using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Architecture.Services
{
    public interface ICustomerCareService
    {
        bool CustomerHasPositivePaymentHistory();
        bool UpdateFidelityCard();
    }
}
