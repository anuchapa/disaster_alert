using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DisasterAlert.context.Entities;

public class RiskReport
{
    public long Id { get; set; }
    public long RegionDisasterId { get; set; }
    public float RiskScore { get; set; }
    public string RiskLevel { get; set; } = string.Empty;
    public bool AlertTrigger { get; set; }

    [ForeignKey(nameof(RegionDisasterId))]
    public RegionDisaster? RegionDisaster { get; set; }
}
