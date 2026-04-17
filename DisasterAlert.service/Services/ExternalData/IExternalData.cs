using System;
using DisasterAlert.service.Dtos.Response;

namespace DisasterAlert.service.Services;

public interface IExternalData
{
    Task<OpenWeatherResponse?> GetCurrentWeatherAsync(double lat, double lon);
    Task<EarthquakeResponse?> GetEarthquakeDataAsync(double lat, double lon);
}
