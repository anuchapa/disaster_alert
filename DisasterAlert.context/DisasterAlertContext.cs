using DisasterAlert.context.Entities;
using Microsoft.EntityFrameworkCore;

namespace DisasterAlert.context;

public class DisasterAlertContext : DbContext
{
    public DbSet<Region> Regions { get; set; }
    public DbSet<RiskReport> RiskReports { get; set; }
    public DbSet<RegionDisaster> RegionDisasters { get; set;}
    public DbSet<DisasterType> DisasterTypes { get; set; }
    public DbSet<User> Users {get; set;}
    public DbSet<UserSubscription> UserSubscriptions {get; set;}
    public DbSet<RiskAlert> RiskAlerts {get; set;}
    public DisasterAlertContext(DbContextOptions<DisasterAlertContext> options) : base(options) { }

}
