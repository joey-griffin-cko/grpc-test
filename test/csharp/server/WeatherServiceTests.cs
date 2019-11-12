using System.Threading.Tasks;
using server.Services;
using Xunit;

namespace server.tests
{
    public class WeatherServiceTests
    {
        [Fact]
        public async Task GetWeather_should_return_WeatherReply()
        {
            var sut = new WeatherService();
            var result = await sut.GetWeather(new WeatherRequest(), null);

            Assert.IsType<WeatherReply>(result);
            Assert.Equal("2", result.Id);
        }
    }
}
