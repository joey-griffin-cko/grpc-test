using System;
using System.Net.Http;
using System.Threading.Tasks;
using client.Models;

namespace client.Services
{
    public sealed class WeatherHttpService
    {
        public HttpClient Client { get; }
        public WeatherHttpService(HttpClient client)
        {
            client.BaseAddress = new Uri("https://localhost:5001/");
            Client = client;
        }

        public async Task<HttpResponseMessage> GetWeatherAsync(WeatherRequest request)
        {
            var response = await Client.GetAsync("weatherforecast");

            response.EnsureSuccessStatusCode();

            return response;
        }
    }
}