
using Microsoft.AspNetCore.Hosting;
using Project.API.Configuration;
using Project.Application.Configuration.Validation;
using Project.Domain.SeedWork;
using Serilog;
using Serilog.Formatting.Compact;
using System.Runtime.CompilerServices;
using Hellang.Middleware.ProblemDetails;
using Project.API.SeedWork;
using Project.Application.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Project.Application.Configuration.Emails;
using Project.Infrastructure.Caching;
using Project.Infrastructure;

namespace Project.API
{
    public class Program
    {
        private static Serilog.ILogger _logger;

        private const string OrdersConnectionString = "OrdersConnectionString";
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            _logger = ConfigureLogger();
            // Add services to the container.
            _logger.Information("Logger configured");

            AddCustomeService(builder.Services);
            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
           
            var app = builder.Build();
            app.UseMiddleware<CorrelationMiddleware>();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseProblemDetails();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();
            app.UseSwaggerDocumentation();
            app.Run();
        }

        private static Serilog.ILogger ConfigureLogger()
        {
            return new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{Context}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.RollingFile(new CompactJsonFormatter(), "logs/logs")
                .CreateLogger();
        }
        public static IServiceCollection AddCustomeService(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSwaggerDocumentation();

            services.AddProblemDetails(x =>
            {
                x.Map<InvalidCommandException>(ex => new InvalidCommandProblemDetails(ex));
                x.Map<BusinessRuleValidationException>(ex => new BusinessRuleValidationExceptionProblemDetails(ex));
            });
            services.AddHttpContextAccessor();
            var serviceProvider = services.BuildServiceProvider();
            var _configuration = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IConfiguration>();
            IExecutionContextAccessor executionContextAccessor = new ExecutionContextAccessor(serviceProvider.GetService<IHttpContextAccessor>());
            var children = _configuration.GetSection("Caching").GetChildren();
            var cachingConfiguration = children.ToDictionary(child => child.Key, child => TimeSpan.Parse(child.Value));
            var emailsSettings = _configuration.GetSection("EmailsSettings").Get<EmailsSettings>();
            var memoryCache = serviceProvider.GetService<IMemoryCache>();
            return services.Initialize(
                    _configuration[OrdersConnectionString],
                    new MemoryCacheStore(memoryCache, cachingConfiguration),
                    null,
                    emailsSettings,
                    _logger,
                    executionContextAccessor
                );
            //return ApplicationStartup.Initialize(
            //    services,
            //    _configuration[OrdersConnectionString],
            //    new MemoryCacheStore(memoryCache, cachingConfiguration),
            //    null,
            //    emailsSettings,
            //    _logger,
            //    executionContextAccessor);
        }
    }
   
}