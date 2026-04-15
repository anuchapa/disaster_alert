using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DisasterAlert.context.Entities;

[Index(nameof(RegionId), nameof(DisasterTypeId), IsUnique = true, Name = "idx_re_dis")]
public class RegionDisaster
{
    public long Id { get; set; }

    public required long RegionId { get; set; }

    public required long DisasterTypeId { get; set; }

    public float? Threshold { get; set; }

    [ForeignKey(nameof(RegionId))]
    public Region? Region { get; set; }

    [ForeignKey(nameof(DisasterTypeId))]
    public DisasterType? DisasterType { get; set; }
}
