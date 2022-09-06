using Microsoft.AspNetCore.Mvc;
using Orleans;
using Sample.Grains.Interfaces;
using Sample.Grains.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Silo.Api
{
    [ApiController]
    [ApiVersion("1")]
    [Route("api/[controller]")]
    public class WeatherController:ControllerBase
    {
        private readonly IGrainFactory factory;

        public WeatherController(IGrainFactory factory)
        {
            this.factory = factory;
        }

        [HttpGet]
        public Task<ImmutableArray<WeatherInfo>> GetAsync() =>
            factory.GetGrain<IWeatherGrain>(Guid.Empty).GetForecastAsync();
    }
}
