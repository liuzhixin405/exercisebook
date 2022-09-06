using NUnit.Framework;

namespace TestRestSharpComponent
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
          
            IWeatherForecastApi api = new WeatherForecastApi("http://localhost:9001");
            var res = api.ListWeatherForecastAsync().GetAwaiter().GetResult();
            var result = res.Data;
            Assert.IsNotEmpty(result);

        }
    }
}