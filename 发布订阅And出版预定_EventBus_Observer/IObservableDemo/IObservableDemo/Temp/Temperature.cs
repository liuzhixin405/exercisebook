using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IObservableDemo.Temp
{
    internal class Temperature
    {
        private decimal temp;
        private DateTime tempDate;
        public Temperature(decimal temperature,DateTime dateAndtime)
        {
            this.temp = temperature;
            this.tempDate = dateAndtime;
        }

        public decimal Degrees { get => temp; set => temp = value; }
        public DateTime Date { get => tempDate; set => tempDate = value; }
    }
}
