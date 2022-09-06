using Grpc.Net.Client;
using GrpcGreeterClient;

using var channel = GrpcChannel.ForAddress("https://localhost:7182");




{
    var client = new Greeter.GreeterClient(channel);
    var order = await client.GetOrderByIdAsync(new OrderRequest { OrderId = 123 });
    var reply = await client.SayHelloAsync(new HelloRequest { Name = "GreeterClient" });

    Console.WriteLine($"Greeting:  {reply.Message}");
    Console.WriteLine($"order:  {order.Json}");
}

{
    var clientWf = new WeatherForecaster.WeatherForecasterClient(channel);
    var weatherForecastDetail = await clientWf.GetWeatherForecastAsync(new Google.Protobuf.WellKnownTypes.Empty { });
    var weatherForecast = await clientWf.GetWeatherForecastByIdAsync(new WeatherForecastRequest { WeatherFId = 1 });

    var weatherForecasts = await clientWf.GetWeatherForecastsAsync(new Google.Protobuf.WellKnownTypes.Empty { });
    Console.WriteLine($"weatherForecastDetail:  {weatherForecastDetail.Message}");
    Console.WriteLine("*********************************************************");
    Console.WriteLine($"weatherForecast:   {System.Text.Json.JsonSerializer.Serialize(weatherForecast)}");
    Console.WriteLine("*********************************************************");
    Console.WriteLine($"weatherForecasts:   {weatherForecasts}");

}


Console.WriteLine("Press any key to exit...");
Console.ReadKey();