using AdventureGrainInterfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using var host = Host.CreateDefaultBuilder(args)
    .UseOrleansClient(clientBuilder =>
        clientBuilder.UseLocalhostClustering())
    .Build();

await host.StartAsync();

var client = host.Services.GetRequiredService<IClusterClient>();

var message = client.GetGrain<IFakeMessage>(0);
Console.WriteLine($"获取消息： {await message.GetMessage()}");


try
{
    Console.Read();
}
finally
{
   
    await host.StopAsync();
}
