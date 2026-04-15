using System;
using Microsoft.EntityFrameworkCore;

namespace DisasterAlert.context.Entities;

[Index(nameof(Latitude), nameof(Longitude), IsUnique = true, Name = "idx_lat_lon")]
[Index(nameof(RegionId), IsUnique = true)]
public class Region
{
    public long Id { get; set; }
    public required string RegionId { get; set; } 

    public double Latitude { get; set; }

    public double Longitude { get; set; }
}
