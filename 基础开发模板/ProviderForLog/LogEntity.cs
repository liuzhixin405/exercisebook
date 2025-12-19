using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProviderForLog
{
    internal class LogEntity
    {
        public LogType Type { get; set; }

        public LogLevel Levle { get; set; }
        public LogContent Content { get; set; }
    }
}
