using Quartz.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crawler.QuartzNet.CustomLog
{
    public class CustomConsoleLogProvider : ILogProvider
    {
        public Quartz.Logging.Logger GetLogger(string name)
        {
            return new Quartz.Logging.Logger((level, func, exception, parameters) =>
            {
                if (level >= LogLevel.Info && func != null)
                {
                    Console.WriteLine($"[{DateTime.Now.ToLongDateString()}][{level}]{func()}{string.Join(";", parameters.Select(p => p == null ? "" : p.ToString()))}自定义日志{name}");
                }
                return true;
            });
        }

        public IDisposable OpenMappedContext(string key, object value, bool destructure = false)
        {
            return new LoggerDisposed();
        }

        public IDisposable OpenNestedContext(string message)
        {
            return new LoggerDisposed();
        }
    }
}
