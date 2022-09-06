using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Crawler.Logger
{
    public class LoggerHelper
    {
        private static readonly ILog logger;

        static LoggerHelper()
        {
            if (logger == null)
            {
                var respository = LogManager.CreateRepository("CrawlerRepository");
                var log4netConfig = new FileInfo("Logger/log4net.config");
                XmlConfigurator.Configure(respository, log4netConfig);
                logger = LogManager.GetLogger(respository.Name, "InfoLogger");
            }
        }
        #region Utilities
        /// <summary>
        /// 普通日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void Info(string message, Exception exception = null)
        {
            if (exception == null)
                logger.Info(message);
            else
                logger.Info(message, exception);
        }

        /// <summary>
        /// 告警日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void Warn(string message, Exception exception = null)
        {
            if (exception == null)
                logger.Warn(message);
            else
                logger.Warn(message, exception);
        }

        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void Error(string message, Exception exception = null)
        {
            if (exception == null)
                logger.Error(message);
            else
                logger.Error(message, exception);
        }
        #endregion
    }
}
