using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace DiagnosticDemo.Diagnostics
{
    public class KeyValueObserver : IObserver<KeyValuePair<string, object>>
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly int _minCommandElapsedMilliseconds;
        private static readonly ConcurrentDictionary<string, StackTrace> _commandStackTraceDic
            = new ConcurrentDictionary<string, StackTrace>();
        public KeyValueObserver(ILoggerFactory loggerFactory, int minCommandElapsedMilliseconds)
        {
            _loggerFactory = loggerFactory;
            _minCommandElapsedMilliseconds = minCommandElapsedMilliseconds;
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(KeyValuePair<string, object> value)
        {
            var logger = _loggerFactory?.CreateLogger(GetType());

            LogLevel logLevel = LogLevel.Information;

            Exception ex = null;
            if (value.Key == BeforeActionEventData.EventName)
            {
                _commandStackTraceDic[BeforeActionEventData.EventName.ToLower()] = new StackTrace(true);
            }
            if (value.Key == BeforeActionEventData.EventName)
            {
                var beforeActionEventData = value.Value as BeforeActionEventData;
                using var scop = logger.BeginScope(new Dictionary<string, object>
                    {
                        { "StackTrace",_commandStackTraceDic[BeforeActionEventData.EventName.ToLower()]}
                    });

                var message = "this is  diagnostics ";

                logger?.Log(
                      logLevel,
                        ex, message,
                        beforeActionEventData.RouteData
                       );
            }
            _commandStackTraceDic.TryRemove(BeforeActionEventData.EventName.ToLower(), out _);
        }
    }
}
