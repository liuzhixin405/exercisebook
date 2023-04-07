using System;
using System.Diagnostics;
using System.Transactions;

namespace LogTemplate
{
    #region MyRegion
    //internal class Program
    //{
    //    private static Random _random;
    //    private static string _template;
    //    private static ILogger _logger;
    //    private static Action<ILogger, int, long, double, TimeSpan, Exception> _log;
    //    static async Task Main(string[] args)
    //    {
    //        _random = new Random();
    //        _template = "method foobarasync is invoked." +
    //            "\n\t\tArgument: foo={foo},bar={bar}" +
    //            "\n\t\tReturn value:{returnValue}" +
    //            "\n\t\tTime:{time}";
    //        _log = LoggerMessage.Define<int, long, double, TimeSpan>(LogLevel.Trace, 3721, _template);
    //        _logger = new ServiceCollection().AddLogging(builder=>builder.SetMinimumLevel(LogLevel.Trace)
    //            .AddConsole()).BuildServiceProvider().GetRequiredService<ILogger<Program>>();

    //        await FoobarAsync(_random.Next(),_random.Next());
    //        await FoobarAsync(_random.Next(), _random.Next());
    //        Console.Read();
    //    }

    //    static async Task<double> FoobarAsync(int foo,long bar)
    //    {
    //        var stopwatch = Stopwatch.StartNew();
    //        await Task.Delay(_random.Next(100, 900));
    //        var result = _random.Next();
    //        _log(_logger, foo, bar, result, stopwatch.Elapsed, null);
    //        return result;
    //    }
    //} 
    #endregion

    internal class Program
    {
       
        static async Task Main(string[] args)
        {
           
           var logger = new ServiceCollection().AddLogging(builder => builder.SetMinimumLevel(LogLevel.Trace)
                .AddConsole(ops=>ops.IncludeScopes=true)).BuildServiceProvider().GetRequiredService<ILogger<Program>>();

            var scopeFactory = LoggerMessage.DefineScope<Guid>("Foobar Transaction[{TransactionId}]");
            var opetionCompleted = LoggerMessage.Define<String, TimeSpan>(LogLevel.Trace, 3721, "Operation {options} complete at {time}");

            using (scopeFactory(logger, Guid.NewGuid()))
            {
                await InvokeAsync();
            }
            using (scopeFactory(logger, Guid.NewGuid()))
            {
                await InvokeAsync();
            }
            Console.Read();

            async Task InvokeAsync()
            {
                var stopwatch = Stopwatch.StartNew();
                await Task.Delay(500);
                opetionCompleted(logger, "foo", stopwatch.Elapsed, null);
                await Task.Delay(500);
                opetionCompleted(logger, "bar", stopwatch.Elapsed, null);
                await Task.Delay(500);
                opetionCompleted(logger, "baz", stopwatch.Elapsed, null);
            }
        }

       
    }
}