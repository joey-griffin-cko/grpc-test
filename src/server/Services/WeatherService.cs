using System;
using System.Linq;
using System.Threading.Tasks;

namespace server.Services
{
    public sealed class WeatherService : Weather.WeatherBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public override Task<WeatherReply> GetWeather(WeatherRequest request, Grpc.Core.ServerCallContext context)
        {
            var rng = new Random();
            var details = Enumerable.Range(1, 5).Select(index => new Details
            {
                Date = DateTime.Now.AddDays(index).ToString(),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();

            var response = new WeatherReply { Id = "2" };
            response.Details.AddRange(details);

            return Task.FromResult(response);
        }

        public override async Task GetWeatherStream(Google.Protobuf.WellKnownTypes.Empty request, Grpc.Core.IServerStreamWriter<WeatherReply> responseStream, Grpc.Core.ServerCallContext context)
        {
            var rng = new Random();
            var i = 0;

            while (!context.CancellationToken.IsCancellationRequested && i < 20)
            {
                await Task.Delay(500); // Gotta look busy

                var details = new Details
                {
                    Date = DateTime.UtcNow.ToString(),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                };

                var response = new WeatherReply { Id = $"{i + 1}", Someprop = "" };
                response.Details.Add(details);

                await responseStream.WriteAsync(response);
                i++;
            }
        }
    }
}