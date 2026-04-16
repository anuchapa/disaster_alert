using System;
using DisasterAlert.service.Dtos.Response;

namespace DisasterAlert.service.Dtos.model;

public class WeatherRegionModel
{
    public string RegionId { get; set; } = string.Empty;
    public  OpenWeatherResponse? WeatherRespones {get; set;}
    public  EarthquakeResponse? EarthquakeRespones {get; set;}
    public List<string> ErrorMessage {get; set;} = [];

}
