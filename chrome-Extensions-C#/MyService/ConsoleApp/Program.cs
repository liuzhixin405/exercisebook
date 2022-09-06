using log4net;
using log4net.Config;
using log4net.Core;
using log4net.Repository;
using System;
using System.IO;


namespace ConsoleApp
{
    class Program
    {
        private static ILog log;
        [STAThread]
        static void Main(string[] args)
        {
            ILoggerRepository repository = LoggerManager.CreateRepository("NETCoreRepository");
            XmlConfigurator.Configure(repository, new FileInfo("log4net.config"));//文件日志
            log = LogManager.GetLogger(repository.Name, "NETCorelog4net");
            Console.WriteLine("Hello World!");

            if (args.Length != 0)
            {
                string chromeMessage = OpenStandardStreamIn();
                log.Info("--------------------My application starts with Chrome Extension message: " + chromeMessage + "---------------------------------");
            }
            Console.Read();
        }

        private static string OpenStandardStreamIn()
        {
            //We need to read first 4 bytes for length information

           Stream stdin = Console.OpenStandardInput();
            log.Info($"stdin= {stdin}");
            int length = 0;
            byte[] bytes = new byte[4];
            stdin.Read(bytes, 0, 4);
            length = System.BitConverter.ToInt32(bytes, 0);

            string input = "";
            for (int i = 0; i < length; i++)
            {
                input += (char)stdin.ReadByte();
            }
            log.Info($"input= {input}");
            return input;
        }
    }
}