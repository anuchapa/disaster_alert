using System;

namespace DisasterAlarm.service.Services;

public class ExternalApiSetting
{
    public string OpenWeatherUrl { get; set; } = string.Empty;
    public string OpenWeatherApiKey { get; set; } = string.Empty;
    public string EarthquakeUrl { get; set; } = string.Empty;
}
