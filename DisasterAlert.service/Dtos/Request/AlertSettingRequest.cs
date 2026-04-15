using System;

namespace DisasterAlert.service.Dtos.Request;

public class AlertSettingRequest
{
    public string RegionId { get; set; } = string.Empty;
    public string DisasterType { get; set; } = string.Empty;
    public float ThresholdScore { get; set; }
}
