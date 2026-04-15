using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DisasterAlert.context.Entities;

[Index(nameof(RegionId), nameof(UserId), IsUnique = true, Name = "idx_user_sub")]
public class UserSubscription
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public long RegionId { get; set; }
    
    [ForeignKey(nameof(RegionId))]
    public Region? Region { get; set; }

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }
}
