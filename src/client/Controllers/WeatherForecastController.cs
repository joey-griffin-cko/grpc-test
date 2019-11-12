using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using client.Services;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using server.Services;

namespace client.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly Weather.WeatherClient _weatherClient;
        private readonly WeatherHttpService _weatherService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, Weather.WeatherClient weatherClient, WeatherHttpService weatherService)
        {
            _logger = logger;
            _weatherClient = weatherClient;
            _weatherService = weatherService;
        }

        [HttpGet("grpc")]
        public async Task<IActionResult> GetWithGrpc(int runs)
        {
            var responseTimeMs = 0.0;
            var stopWatch = new Stopwatch();

            for (var i = 0; i < runs; i++)
            {
                stopWatch.Reset();
                stopWatch.Start();
                var reply = await _weatherClient.GetWeatherAsync(new server.Services.WeatherRequest());
                stopWatch.Stop();

                var ts = stopWatch.Elapsed;
                responseTimeMs += ts.Milliseconds;
            }

            var response = new { avgResponseTime = responseTimeMs / runs };

            return Ok(response);
        }

        [HttpGet("grpc-stream")]
        public async Task<IActionResult> GetGrpcStream()
        {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(50));
            using var streamingCall = _weatherClient.GetWeatherStream(new Empty(), cancellationToken: cts.Token);

            try
            {
                await foreach (var weatherReply in streamingCall.ResponseStream.ReadAllAsync(cancellationToken: cts.Token))
                {
                    Console.WriteLine($"{weatherReply.Details[0].Date} / {weatherReply.Details[0].TemperatureC}C / {weatherReply.Details[0].Summary}");
                }
            }
            catch (RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.Cancelled)
            {
                Console.WriteLine("Stream cancelled.");
            }

            return Ok();
        }

        [HttpGet("ngrpc")]
        public async Task<IActionResult> GetWithoutGrpc(int runs)
        {
            var responseTimeMs = 0.0;
            var stopWatch = new Stopwatch();

            for (var i = 0; i < runs; i++)
            {
                stopWatch.Reset();
                stopWatch.Start();
                var reply = await _weatherService.GetWeatherAsync(new Models.WeatherRequest());
                stopWatch.Stop();

                var ts = stopWatch.Elapsed;
                responseTimeMs += ts.Milliseconds;
            }

            var response = new { avgResponseTime = responseTimeMs / runs };

            return Ok(response);
        }
    }
}
