using System;
using DisasterAlarm.service.Dtos.Response;

namespace DisasterAlarm.service.Services;

public interface ExternalData
{
    Task<OpenWeatherResponse?> GetCurrentWeatherAsync(double lat, double lon);
    Task<EarthquakeResponse?> GetEarthquakeDataAsync(double lat, double lon);
}
