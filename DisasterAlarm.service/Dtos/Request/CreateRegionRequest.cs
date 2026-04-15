using System;

namespace DisasterAlarm.service.Dtos.Request;

public class CreateRegionRequest
{
    public required string RegionId { get; set; } = string.Empty;
    public required Location LocationCoordinates {get; set;}
    public required List<string> DisasterTypes { get; set; } = [];
}

public class Location
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}