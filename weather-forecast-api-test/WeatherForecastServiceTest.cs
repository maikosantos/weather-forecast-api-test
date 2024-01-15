using weather_forecast_api.Services;
using weather_forecast_api.Dto;
using weather_forecast_api;
using Moq;
using Microsoft.Extensions.Logging;
using Moq.Protected;
using System.Net;
using weather_forecast_api_test.weather_forecast_api_test;
using Newtonsoft.Json;
using System.Text;

namespace weather_forecast_api_test
{
    public class WeatherForecastServiceTest
    {
        private readonly Mock<ILogger<WeatherForecastService>> _mockLogger = new Mock<ILogger<WeatherForecastService>>();
        private readonly Mock<DelegatingHandler> _httpClientHandler = new();
        private readonly Mock<IOptions> _mockOption = new Mock<IOptions>();
        private readonly Mock<HttpClient> _mockHttpClient = new Mock<HttpClient>();
        private readonly IWeatherForecastService _weatherForecastService;

        public WeatherForecastServiceTest()
        {
            var httpClient = new HttpClient(_httpClientHandler.Object);
            _weatherForecastService = new WeatherForecastService(httpClient, _mockOption.Object, _mockLogger.Object);
        }

        [Fact(DisplayName = "Return Coordinates - Success")]
        public async Task GetCoordinatesByAddressSuccess()
        {
            //Arrange   
            _mockOption.SetupGet(o=> o.GeocoderApiUrl).Returns("https://geocoding.geo.census.gov/geocoder");
            _mockOption.SetupGet(o => o.WeatherApiUrl).Returns("https://api.weather.gov");
           

            var address = new Address()
            {
                Street = "1600 Pennsylvania Ave NW",
                City = "Washington",
                StateAbbreviation = "DC",
                ZipCode = "20500"
            };

            //Act
            var setupApiRequest = _httpClientHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());

            var apiMockedResponse = setupApiRequest
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(MockWeatherResponse.CreateWeatherResponse()), Encoding.UTF8, "application/json")
                });

            var result = await _weatherForecastService.GetCoordinatesByAddress(address);

            //Assert    
            Assert.NotNull(result);
            Assert.Equal(10, result.x);
            Assert.Equal(10, result.y);

        }

        [Fact(DisplayName = "Return Coordinates - Failure")]
        public async Task GetCoordinatesByAddressFailure()
        {
            //Arrange   
            _mockOption.SetupGet(o => o.GeocoderApiUrl).Returns("https://geocoding.geo.census.gov/geocoder");
            _mockOption.SetupGet(o => o.WeatherApiUrl).Returns("https://api.weather.gov");

            var address = new Address()
            {
                Street = "1600 Pennsylvania Ave NW",
                City = "Washington",
                StateAbbreviation = "DC",
                ZipCode = "20500"
            };

            //Act
            var setupApiRequest = _httpClientHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());

            var apiMockedResponse = setupApiRequest
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Content = new StringContent("Error")
                });

            var ex = await Assert.ThrowsAsync<Exception>(async () => await _weatherForecastService.GetCoordinatesByAddress(address));

            //Assert 
            Assert.Contains("Error getting coordinates for address:", ex.Message);

        }
    }
}