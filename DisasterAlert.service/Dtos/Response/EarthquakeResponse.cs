using System;

namespace DisasterAlert.service.Dtos.Response;

public class EarthquakeResponse
{
    public string Type { get; set; } = string.Empty;
    public Metadata Metadata { get; set; } = new();
    public List<Feature> Features { get; set; } = new();
    public List<double> Bbox { get; set; } = new();
}

public class Metadata
{
    public long Generated { get; set; }
    public string Url { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int Status { get; set; }
    public string Api { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class Feature
{
    public string Type { get; set; } = string.Empty;
    public Properties Properties { get; set; } = new();
    public Geometry Geometry { get; set; } = new();
    public string Id { get; set; } = string.Empty;
}

public class Properties
{
    public double Mag { get; set; }
    public string Place { get; set; } = string.Empty;
    public long Time { get; set; }
    public long Updated { get; set; }
    public object? Tz { get; set; }
    public string Url { get; set; } = string.Empty;
    public string Detail { get; set; } = string.Empty;
    public object? Felt { get; set; }
    public object? Cdi { get; set; }
    public object? Mmi { get; set; }
    public object? Alert { get; set; }
    public string Status { get; set; } = string.Empty;
    public int Tsunami { get; set; }
    public int Sig { get; set; }
    public string Net { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Ids { get; set; } = string.Empty;
    public string Sources { get; set; } = string.Empty;
    public string Types { get; set; } = string.Empty;
    public int? Nst { get; set; }
    public double? Dmin { get; set; }
    public double? Rms { get; set; }
    public int? Gap { get; set; }
    public string MagType { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
}

public class Geometry
{
    public string Type { get; set; } = string.Empty;
    public List<double> Coordinates { get; set; } = new();
}
