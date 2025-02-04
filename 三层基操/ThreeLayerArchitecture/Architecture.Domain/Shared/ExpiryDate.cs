﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Architecture.Domain.Shared
{
    public sealed class ExpiryDate
    {
        public static ExpiryDate Unknown = new ExpiryDate(0, 0);

        public ExpiryDate(int month,int year)
        {
            if(month < 1 || month > 12)
            {
                When= DateTime.MinValue;
                return;
            }
            var thisYear = DateTime.Today.Year;
            if(year <thisYear || year > thisYear +5) {         
            When= DateTime.MinValue; return;
            }
            When = new DateTime(year, month, 28);
            Month = month;
            Year = year;
        }

        private ExpiryDate()
        {
        }
        public int Month { get; private set; }
        public int Year { get; private set; }
        public DateTime When { get; private set; }
        public override string ToString()
        {
            return String.Format("{0}", When.ToShortDateString());
        }

        #region Equality
        public override bool Equals(object obj)
        {
            if (this == (ExpiryDate)obj)
                return true;
            if (obj == null || GetType() != obj.GetType())
                return false;

            var other = (ExpiryDate)obj;
            return When == other.When;
        }
        public override int GetHashCode()
        {
            return When.GetHashCode();
        }
        #endregion
    }
}
