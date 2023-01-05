using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


// Configure the host
using var host = Host.CreateDefaultBuilder(args)
    .UseOrleans(siloBuilder =>
    {
        siloBuilder.UseLocalhostClustering();
    })
    .Build();

// Start the host
await host.StartAsync();

Console.WriteLine("Press any key to exit.");
Console.ReadKey();
await host.StopAsync();

return 0;
