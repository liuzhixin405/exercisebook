using Dapr.Client;
using DaprLogger.Model;
using Microsoft.AspNetCore.Mvc;

namespace DaprLoggerClient
{
    public class DLogger : ILogger
    {
        private readonly Config _configuration;
        
        public DLogger(Config configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= LogLevel.Trace;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            var daprClient = new DaprClientBuilder().Build();
            try
            {
                daprClient.PublishEventAsync(_configuration.PubSubComponent,_configuration.LoggerTopic,
                    System.Text.Json.JsonSerializer.Deserialize<LogData>(formatter(state,exception))).ConfigureAwait(false).GetAwaiter().GetResult();
            }catch(Exception ex)
            {

            }
        }
    }
    public class Config
    {
        public string PubSubComponent { get; set; } = "pubsub";
        public string LoggerTopic { get; set; } = "logging";
    }
}
