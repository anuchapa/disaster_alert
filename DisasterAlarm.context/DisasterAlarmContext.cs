using DisasterAlarm.context.Entities;
using Microsoft.EntityFrameworkCore;

namespace DisasterAlarm.context;

public class DisasterAlarmContext : DbContext
{
    public DbSet<Region> Regions { get; set; }
    public DbSet<RiskReport> RiskReports { get; set; }
    public DbSet<RegionDisaster> RegionDisasters { get; set; }
    public DbSet<DisasterType> DisasterTypes { get; set; }
    public DbSet<User> Users {get; set;}
    public DbSet<UserSubscription> UserSubscriptions {get; set;}
    public DisasterAlarmContext(DbContextOptions<DisasterAlarmContext> options) : base(options) { }

}
