using System;
using System.Text.Json.Serialization;

namespace DisasterAlert.service.Dtos.Response;

public class OpenWeatherResponse
{
    public double Lat { get; set; }
    public double Lon { get; set; }
    public string Timezone { get; set; } = string.Empty;
    public int TimezoneOffset { get; set; }
    public CurrentWeather Current { get; set; } = new();
    //public List<MinutelyData> Minutely { get; set; } = new();

    public string ErrorMessage = string.Empty;
    public bool Success = false;
}

public class CurrentWeather
{
    public int Dt { get; set; }
    public int Sunrise { get; set; }
    public int Sunset { get; set; }
    public double Temp { get; set; }
    public double FeelsLike { get; set; }
    public int Pressure { get; set; }
    public int Humidity { get; set; }
    public double DewPoint { get; set; }
    public double Uvi { get; set; }
    public int Clouds { get; set; }
    public int Visibility { get; set; }
    public double WindSpeed { get; set; }
    public int WindDeg { get; set; }
    public List<WeatherInfo> Weather { get; set; } = new();
    public Rain1H? Rain { get; set; } = new();
}

public class WeatherInfo
{
    public int Id { get; set; }
    public string Main { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
}

public class Rain1H
{
    [JsonPropertyName("1h")]
    public double H1 { get; set; }
}

// public class MinutelyData
// {
//     public int Dt { get; set; }
//     public double Precipitation { get; set; }
// }
