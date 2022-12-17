using Project.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.Payments
{
    public class PaymentId:TypedIdValueBase
    {
        public PaymentId(Guid value):base(value)
        {

        }
    }
}
