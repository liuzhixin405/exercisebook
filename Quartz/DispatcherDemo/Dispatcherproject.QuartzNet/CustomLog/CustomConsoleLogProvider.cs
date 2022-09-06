using Quartz.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dispatcherproject.QuartzNet.CustomLog
{
    public class CustomConsoleLogProvider : ILogProvider
    {
        public Logger GetLogger(string name)
        {
            return new Logger((level, func, exception, parameters) =>
            {
                if (level >= LogLevel.Info && func != null)
                {
                    Console.WriteLine($"[{DateTime.Now.ToLongDateString()}][{level}]{func()}{string.Join(";", parameters.Select(p=>p==null?"":p.ToString()))}自定义日志{name}");
                }
                return true;
            });
        }

        public IDisposable OpenMappedContext(string key, object value, bool destructure = false)
        {
            throw new NotImplementedException();
        }

        public IDisposable OpenNestedContext(string message)
        {
            throw new NotImplementedException();
        }
    }
}
