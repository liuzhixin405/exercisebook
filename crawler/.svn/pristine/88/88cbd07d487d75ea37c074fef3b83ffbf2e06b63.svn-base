using Crawler.Logger;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Crawler.Common
{
    public class CommonHelper
    {
       
        public static bool IsUrl(string str)
        {
            try
            {
                string Url = @"^http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?$";
                return Regex.IsMatch(str, Url);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        /// <summary>
        /// 解析cookie返回CookieInfoOptions对象
        /// </summary>
        /// <param name="strs"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ParsingCookie(string strs)
        {
            Dictionary<string, string> keyValues = new Dictionary<string, string>();
            var json = string.Empty;
            var collection = strs.Split(";");
            foreach (var str in collection)
            {
                var arr = str.Split("=");
                arr[0] = arr[0].Trim();
                if (!string.IsNullOrWhiteSpace(arr[1]))
                {
                    keyValues.Add(arr[0], arr[1]);
                }
            }
            return keyValues;
        }
        /// <summary>
        /// 同步服务器时间和当前时间
        /// </summary>
        /// <returns></returns>
        public static string GetSTime(bool iSLocalEnvironment = false)
        {
            
            return iSLocalEnvironment ? DateTime.Now.ToString(): DateTime.Now.AddHours(8).ToString();
        }
        public static void ConsoleAndLogger(string message,LoggerType loggerType)
        {
            var current = ConsoleColor.White;
            switch (loggerType)
            {
                case LoggerType.Error:
                    LoggerHelper.Error(message);
                    current = ConsoleColor.Red;
                    break;            
                case LoggerType.Warn:
                    LoggerHelper.Warn(message);
                    current = ConsoleColor.White;
                    break;
                case LoggerType.Info:
                default:
                    current = ConsoleColor.White;
                    LoggerHelper.Info(message);
                    break;
            }
            ConsoleHelper.WriteLine("", message, "", current);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public enum LoggerType
        {
            Error,
            Info,
            Warn
        }
    }
}
