using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

using weather_forecast_api;
using weather_forecast_api.Controllers;
using weather_forecast_api.Dto;
using weather_forecast_api.Services;

namespace weather_forecast_api_test
{

    public class MockWeatherServiceForecast : IWeatherForecastService
    {
        public async Task<Coordinates> GetCoordinatesByAddress(Address address) => new Coordinates { x = 11, y = 11 };
        public async Task<string> GetForecastUrl(Coordinates coordinates) => "https://test.com";
        public async Task<IActionResult> GetForecast(string forecastUrl) => new OkResult();
    }

    public class WeatherForecastControllerTest
    {
        private readonly Mock<ILogger<WeatherForecastController>> _mockLogger = new Mock<ILogger<WeatherForecastController>>();
        private readonly IWeatherForecastService _weatherForecastService;
        private readonly WeatherForecastController _controller;

        public WeatherForecastControllerTest()
        {
            _weatherForecastService = new MockWeatherServiceForecast();
            _controller = new WeatherForecastController(_mockLogger.Object, _weatherForecastService);
        }
               
        [Fact(DisplayName = "Return GetWeatherForcast - Success")]
        public async Task GetWeatherForcastTest()
        {

            var address = new Address()
            {
                Street = "1600 Pennsylvania Ave NW",
                City = "Washington",
                StateAbbreviation = "DC",
                ZipCode = "20500"
            };

            var result = _controller.GetWeatherForcast(address);

            Assert.NotNull(result);

        }

     }

}