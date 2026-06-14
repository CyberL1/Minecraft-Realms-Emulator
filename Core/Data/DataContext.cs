using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Core.Data;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<AppConfig> AppConfig { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<Realm> Realms { get; set; }
    public DbSet<Slot> Slots { get; set; }
    public DbSet<SlotOptions> SlotOptions { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<WorldSettings> WorldSettings { get; set; }
}
