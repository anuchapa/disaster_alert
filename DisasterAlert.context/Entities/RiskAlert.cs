using System;

namespace DisasterAlert.context.Entities;

public class RiskAlert
{
    public long Id {get; set;}
    public string RegionId { get; set; } = string.Empty;
    public string DisasterType { get; set; } = string.Empty;
    public string AlertMessage { get; set; } = string.Empty;
    public string RiskLevel { get; set; } = string.Empty;
    public DateTime Timestamp {get; set;}
    public bool SuccessStatus {get; set;} = false;
}
