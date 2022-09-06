using Orleans.Concurrency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Grains.Interfaces.Models
{
    [Immutable]
    public class WeatherInfo
    {
        public WeatherInfo(DateTime date, int temperatureC, string summary, int temperatureF)
        {
            Date = date;
            TemperatureC = temperatureC;
            Summary = summary;
            TemperatureF = temperatureF;
        }

        public DateTime Date { get; }
        public int TemperatureC { get; }
        public string Summary { get; }
        public int TemperatureF { get; }
    }
}
