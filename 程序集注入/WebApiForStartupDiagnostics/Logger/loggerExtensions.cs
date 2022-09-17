using Microsoft.AspNetCore.Cors.Infrastructure;
using WebApplication1;

namespace WebApi.LoggerExtensions
{
    public static class LoggerExtensions
    {

        private static readonly Action<ILogger, string, int,Exception> _weatherForecastDeleted;
        private static readonly Action<ILogger,  int, Exception> _weatherForecastDeleteFailed;
        private static Func<ILogger, int, IDisposable> _weatherForecastsDeletedScope;

        static LoggerExtensions()
        {
            _weatherForecastDeleted = LoggerMessage.Define<string,int> (
                LogLevel.Information,
                new EventId(4, nameof(WfDeleted)),
                "WeatherForecast Deleted (WeatherForecast = '{WeatherForecast}' Id = {Id})");
            _weatherForecastDeleteFailed = LoggerMessage.Define <int> (
                LogLevel.Error,
                new EventId(5, nameof(WfDeleteFailed)),
                "WeatherForecast Deleted Failed ( Id = {Id})");

            _weatherForecastsDeletedScope = LoggerMessage.DefineScope<int>("All WeatherForecast Deleted (Count = {Count})");
        }


        public static void WfDeleted(this ILogger logger,string wf,int id)
        {
            _weatherForecastDeleted(logger, wf,id, null);
        }

        public static void WfDeleteFailed(this ILogger logger,int id, Exception  ex)
        {
            _weatherForecastDeleteFailed(logger,id, ex);
        }

        public static IDisposable _wfsDeletedScope(this ILogger logger, int count)
        {
            return _weatherForecastsDeletedScope(logger, count);
        }
    }
}
