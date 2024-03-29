using System;

namespace client.Models
{
    public sealed class WeatherResponse
    {
        public DateTime Date { get; set; }
        public int TemperatureC { get; set; }
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
        public string Summary { get; set; }
    }

    public sealed class WeatherRequest
    {
        
    }
}