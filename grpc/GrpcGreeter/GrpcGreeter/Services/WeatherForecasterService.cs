using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using System.Collections.Generic;

namespace GrpcGreeter.Services
{
    public class WeatherForecasterService : WeatherForecaster.WeatherForecasterBase
    {
        private readonly ILogger<WeatherForecasterService> _logger;
        public WeatherForecasterService(ILogger<WeatherForecasterService> logger)
        {
            _logger = logger;
        }
        private static readonly string[] Summaries = new[]
       {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private static readonly List<WeatherForecastDetailResponse> list = Enumerable.Range(1, 5).Select(index => new WeatherForecastDetailResponse
        {
            WeatherFId = index,
            Date = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        }).ToList();
        public override Task<WeatherForecastDetailResponse> GetWeatherForecastById(WeatherForecastRequest request, ServerCallContext context)
        {
            return Task.FromResult(list.Where(x => x.WeatherFId.Equals(request.WeatherFId)).FirstOrDefault() ?? new WeatherForecastDetailResponse
            {

            });

        }
        public override Task<WeatherForecastResponse> GetWeatherForecast(Google.Protobuf.WellKnownTypes.Empty request, ServerCallContext context)
        {
            return Task.FromResult(new WeatherForecastResponse
            {
                Message = System.Text.Json.JsonSerializer.Serialize(list)
            });
        }

        public override Task<WeatherForecastListResponse> GetWeatherForecasts(Empty request, ServerCallContext context)
        {
            var result = new WeatherForecastListResponse()
            {
                Total = (double)list.Count
            };
            list.ForEach(wh => result.WeatherForecasts.Add(new WeatherForecastDetailResponse
            {
                Date = wh.Date,
                Summary = wh.Summary,
                TemperatureC = wh.TemperatureC,
                TemperatureF = wh.TemperatureF,
                WeatherFId = wh.WeatherFId
            }));
            return Task.FromResult(result);
        }

    }
}
