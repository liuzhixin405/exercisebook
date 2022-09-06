using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Crawler.Common
{
    public class ConsoleHelper
    {
        public static void Write(string title, string message, string subTitle, ConsoleColor consoleColor)
        {
            Console.ForegroundColor = consoleColor;
            string dateNowTxt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            Console.Write(string.IsNullOrWhiteSpace(subTitle)?$"[{dateNowTxt}]{title}:{message}":$"[{dateNowTxt}]{title}[{subTitle}]:{message}");
        }

        public static void WriteLine(string title, string message, string subTitle, ConsoleColor consoleColor)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = consoleColor;
            string dateNowTxt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            Console.WriteLine(string.IsNullOrWhiteSpace(subTitle) ? $"[{dateNowTxt}]{title}:{message}" : $"[{dateNowTxt}]{title}[{subTitle}]:{message}");
            Console.ForegroundColor = color;
        }

        public static void ServerWrite(string message,string subTitle=null,ConsoleColor consoleColor = ConsoleColor.White)
        {
            Write("Server",message,subTitle,consoleColor);
        }

        public static void ServerWriteLine(string message, string subTitle = null,
            ConsoleColor consoleColor = ConsoleColor.White)
        {
            WriteLine("Server",message,subTitle,consoleColor);
        }

        public static void ServerWriteError(Exception exception)
        {
            ServerWriteLine(GetErrorMessage(exception),"错误",ConsoleColor.Red);
        }

        private static string GetErrorMessage(Exception exception)
        {
            string message = $"{exception.Message}";
            if (!string.IsNullOrEmpty(exception.StackTrace)) message += $"\r\n{exception.StackTrace}";
            if (exception is AggregateException aggregateException)
            {
                foreach (Exception innerException in aggregateException.InnerExceptions)
                {
                    Exception ex = innerException;
                    do
                    {
                        message += $"\r\n{GetErrorMessage(ex)}";
                        ex = innerException.InnerException;
                    } while (ex != null);
                }
            }
            else
            {
                if (exception.InnerException != null)
                {
                    exception = exception.InnerException;
                    message += $"\r\n{GetErrorMessage(exception)}";
                }
            }

            return message;
        }
    }
}