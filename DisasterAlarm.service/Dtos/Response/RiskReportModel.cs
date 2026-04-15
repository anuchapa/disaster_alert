using System;

namespace DisasterAlarm.service.Dtos.Response;

public class RiskReportResponse
{
    public string RegionId { get; set; } = string.Empty;
    public string DisasterType { get; set; } = string.Empty;
    public float RiskScore { get; set; }
    public string RiskLevel { get; set; } = string.Empty;
    public bool AlertTrigger { get; set; }

}
