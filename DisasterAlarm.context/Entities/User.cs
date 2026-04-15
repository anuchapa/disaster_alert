using System;
using Microsoft.EntityFrameworkCore;

namespace DisasterAlarm.context.Entities;

[Index(nameof(PhoneNumber), IsUnique = true)]
[Index(nameof(Name), IsUnique = true)]

public class User
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<UserSubscription> userSubscriptions = [];

}
