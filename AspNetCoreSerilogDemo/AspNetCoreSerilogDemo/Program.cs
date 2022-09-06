using AspNetCoreSerilogDemo.Controllers;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Events;
using System.ComponentModel.DataAnnotations;

// Setup serilog in a two-step process. First, we configure basic logging
// to be able to log errors during ASP.NET Core startup. Later, we read
// log settings from appsettings.json. Read more at
// https://github.com/serilog/serilog-aspnetcore#two-stage-initialization.
// General information about serilog can be found at
// https://serilog.net/
Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateBootstrapLogger();

try
{
    Log.Information("Starting the web host");
    var builder = WebApplication.CreateBuilder(args);
    // Full setup of serilog. We read log settings from appsettings.json
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());
    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    app.UseSerilogRequestLogging(configure =>
    {
        configure.MessageTemplate = "HTTP {RequestMethod} {RequestPath} ({UserId}) responded {StatusCode} in {Elapsed:0.0000}ms";
    });
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    #region  Demo
    app.MapGet("/Hello", () => "World");

    app.MapGet("/request-context", (IDiagnosticContext diagnosticContext) =>
    {
        // You can enrich the diagnostic context with custom properties.
        // They will be logged with the HTTP request.
        diagnosticContext.Set("UserId", "someone");
    });

    app.MapPost("/minimal-customers", (Serilog.ILogger logger, CustomerDtoClass customer) =>
    {
        // Manually validate data annotations. Unfortunately, this does not
        // work with records currently. See also https://github.com/dotnet/runtime/issues/47602.
        var context = new ValidationContext(customer, null, null);
        var results = new List<ValidationResult>();
        if (!Validator.TryValidateObject(customer, context, results, true))
        {
            var pd = new ProblemDetails
            {
                Type = "https://demo.api.com/errors/validation-error",
                Title = "Validation Error",
                Status = StatusCodes.Status400BadRequest,
            };
            pd.Extensions.Add("RequestId", results);
            return Results.Json(pd, contentType: "application/problem+json", statusCode: StatusCodes.Status400BadRequest);
        }

        logger.Information("Writing customer {CustomerName} to DB", customer.Name);

        return Results.StatusCode(StatusCodes.Status201Created);
    });

    app.MapGet("/", () => Results.Text(@"<html lang=""en""><body>
        <ul>
            <li><a href=""ping"">Hello</a></li>
            <li><a href=""request-context"">Add context to request</a></li>
            <li><a href=""logDemo/simple"">Some simple log messages</a></li>
            <li><a href=""logDemo/exception"">Handled exception</a></li>
            <li><a href=""logDemo/timing"">Log timing</a></li>
            <li><a href=""logDemo/unhandled-exception"">Unhandled exception</a></li>
        </ul></body></html>", "text/html"));
    #endregion
    app.Run();
}
catch
(Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpexctedly");
}
finally
{
    Log.CloseAndFlush();
}

