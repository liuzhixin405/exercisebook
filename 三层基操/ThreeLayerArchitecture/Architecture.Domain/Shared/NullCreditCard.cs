using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Architecture.Domain.Shared
{
    public class NullCreditCard:CreditCard
    {
        public static NullCreditCard Instance = new NullCreditCard();
    }
}
