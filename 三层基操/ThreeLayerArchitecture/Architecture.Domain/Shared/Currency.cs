﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Architecture.Domain.Shared
{
    public class Currency
    {
        public static Currency Default =new Currency();
        protected Currency(string symbol,string name) {
        Symbol= symbol;
            Name= name;
        }
        protected Currency() { }
        public string Symbol { get; private set; }
        public string Name { get; private set; }


        #region Equality
        public static bool operator ==(Currency c1, Currency c2)
        {
            // Both null or same instance
            if (ReferenceEquals(c1, c2))
                return true;

            // Return false if one is null, but not both 
            if (((object)c1 == null) || ((object)c2 == null))
                return false;

            return c1.Equals(c2);
        }
        public static bool operator !=(Currency c1, Currency c2)
        {
            return !(c1 == c2);
        }
        public override bool Equals(object obj)
        {
            if (this == (Currency)obj)
                return true;
            if (obj == null || GetType() != obj.GetType())
                return false;

            var other = (Currency)obj;
            return string.Equals(Symbol, other.Symbol, StringComparison.InvariantCultureIgnoreCase) &&
                   string.Equals(Name, other.Name, StringComparison.InvariantCultureIgnoreCase);
        }
        public override int GetHashCode()
        {
            const int hashIndex = 307;
            var result = (Symbol != null ? Symbol.GetHashCode() : 0);
            result = (result * hashIndex) ^ (Name != null ? Name.GetHashCode() : 0);
            return result;
        }
        #endregion
    }

    public class UsdCurrency : Currency
    {
        public static UsdCurrency Instance = new UsdCurrency();

        public UsdCurrency()
            : base("$", "USD")
        {
        }
    }

    public class EurCurrency : Currency
    {
        public static EurCurrency Instance = new EurCurrency();

        public EurCurrency()
            : base("€", "EUR")
        {
        }
    }

    public class GbpCurrency : Currency
    {
        public static GbpCurrency Instance = new GbpCurrency();

        public GbpCurrency()
            : base("£", "GBP")
        {
        }
    }
}
