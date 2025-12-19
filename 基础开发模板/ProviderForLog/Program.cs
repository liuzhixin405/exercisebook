namespace ProviderForLog
{
    internal class Program
    {
        static void Main(string[] args)
        {
            LogEntity logEntity = LogEntityFactory.CreateLogEntity("\\创建啥的报错了", logType: LogType.Exception, logLevel: LogLevel.Graveness);
            logEntity.Content.LogTrackInfo = "Program.Main";
            ILogSaveProvider saveProvider = new LogSaveLocalhostProvider();
            saveProvider.SaveLog(logEntity);
            Console.WriteLine("日志保存完成");

        }
    }
}
