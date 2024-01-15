using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using weather_forecast_api.Dto;

namespace weather_forecast_api_test.weather_forecast_api_test
{
    public static class MockWeatherResponse
    {
        public static WeatherResponse CreateWeatherResponse()
        {
            WeatherResponse response = new WeatherResponse
            {
                Result = new Result
                {
                    AddressMatches = 
                        new List<AddressMatch> {
                            new AddressMatch {
                                Coordinates = new Coordinates { x = 10, y = 10 }
                            }
                    }
                }
            };

            return response;
        }
        
    }
}
