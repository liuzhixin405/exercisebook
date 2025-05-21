using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandora.Cigfi.Common
{
    /// <summary>
    /// InfluxDB Options
    /// </summary>
    public class InfluxDBOptions
    {
        public string URL { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }
    }
}
