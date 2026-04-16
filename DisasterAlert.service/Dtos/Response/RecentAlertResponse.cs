using System;

namespace DisasterAlert.service.Dtos.Response;

public class RecentAlertResponse
{
    public string RegionId { get; set; } = string.Empty;
    public string DisasterType { get; set; } = string.Empty;
    public string AlertMessage { get; set; } = string.Empty;
    public string RiskLevel { get; set; } = string.Empty;
    public string Timestamp {get; set;} = string.Empty;
    public bool SuccessStatus {get; set;} = false;
}
