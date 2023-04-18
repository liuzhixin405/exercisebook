using LogLibrary;
using System.Diagnostics;

namespace LoggerService
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            DiagnosticListener.AllListeners.Subscribe(new SubLog());
            builder.Services.AddLogging(op=>
            op.AddConsole());

            var logger = LoggerFactory.Create(o =>
            {
                o.AddConsole();
            }).CreateLogger("Program");               
            logger.LogInformation("hello");
           
            var app = builder.Build();
            app.Run();

        }
    }
}