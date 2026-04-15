using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace DisasterAlarm.context.Entities;

[Index(nameof(Disaster), IsUnique = true)]
public class DisasterType
{
    public long Id { get; set; }

    public required string Disaster { get; set; }

}
