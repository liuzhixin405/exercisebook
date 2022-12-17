using Project.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.SharedKernel
{
    public class MoneyValue:ValueObject
    {
        public decimal Value { get; }
        public string Currency { get; }
        private MoneyValue(decimal value,string currency)
        {
            this.Value = value;
            this.Currency = currency;
        }
        public static MoneyValue Of(decimal value,string currency)
        {
            CheckRule(new MoneyValueMustHaveCurrencyRule(currency));
            return new MoneyValue(value, currency);
        }

        public static MoneyValue Of(MoneyValue moneyValue)
        {
            return new MoneyValue(moneyValue.Value, moneyValue.Currency);
        }
        public static MoneyValue operator +(MoneyValue moneyValueLeft,MoneyValue moneyValueRight)
        {
            CheckRule(new MoneyValueOperationMustBePerformedOnTheSameCurrencyRule(moneyValueLeft,moneyValueRight));
            return new MoneyValue(moneyValueLeft.Value + moneyValueRight.Value, moneyValueLeft.Currency);
        }
        public static MoneyValue operator *(int number, MoneyValue moneyValueRight)
        {
            return new MoneyValue(number * moneyValueRight.Value, moneyValueRight.Currency);
        }

        public static MoneyValue operator *(decimal number, MoneyValue moneyValueRight)
        {
            return new MoneyValue(number * moneyValueRight.Value, moneyValueRight.Currency);
        }
    }
    public static class SumExtensions
    {
        public static MoneyValue Sum<T>(this IEnumerable<T> source,Func<T,MoneyValue> selector)
        {
            return MoneyValue.Of(source.Select(selector).Aggregate((x, y) =>x+ y));
        }
        public static MoneyValue Sum(this IEnumerable<MoneyValue> source)
        {
            return source.Aggregate((x, y) => x + y);
        }
    }
}
