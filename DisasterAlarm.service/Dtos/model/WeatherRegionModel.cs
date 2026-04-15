using System;
using DisasterAlarm.service.Dtos.Response;

namespace DisasterAlarm.service.Dtos.model;

public class WeatherRegionModel
{
    public string RegionId { get; set; } = string.Empty;
    public  OpenWeatherResponse? WeatherRespones {get; set;}
    public  EarthquakeResponse? EarthquakeRespones {get; set;}
}
