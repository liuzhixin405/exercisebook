using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProviderForLog
{
    internal class LogEntityFactory
    {
        public static LogEntity CreateLogEntity(string message,LogType logType,LogLevel logLevel)
        {
            return new LogEntity
            {
                Type = logType,
                Levle = logLevel,
                Content = new LogContent
                {
                    Message = message,
                }
            };
        }
    }
}
