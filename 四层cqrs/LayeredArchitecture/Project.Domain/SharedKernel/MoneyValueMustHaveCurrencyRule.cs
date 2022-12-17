using Project.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.SharedKernel
{
    public class MoneyValueMustHaveCurrencyRule : IBusinessRule
    {
        private readonly string _currency;
        public MoneyValueMustHaveCurrencyRule(string currency)
        {
            _currency= currency;
        }
        public string Message => "Money value musty have currency";

        public bool IsBroken()
        {
           return string.IsNullOrEmpty(_currency);
        }
    }
}
