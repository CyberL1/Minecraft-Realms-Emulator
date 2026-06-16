using Core.Entities;
using Core.Enums;
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
    public DbSet<RealmRegionSelectionPreference> RegionSelectionPreferences { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Realm>().HasOne(realm => realm.Subscription).WithOne(subscription => subscription.Realm)
            .HasForeignKey<Subscription>(subscription => subscription.RealmId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Player>().HasOne(player => player.Realm).WithMany(realm => realm.Players)
            .HasForeignKey(player => player.RealmId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Slot>().HasOne(slot => slot.Realm).WithMany(realm => realm.Slots)
            .HasForeignKey(slot => slot.RealmId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Realm>().HasOne(realm => realm.ActiveSlot).WithOne()
            .HasForeignKey<Realm>(realm => realm.ActiveSlotId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Slot>().HasOne(slot => slot.Options).WithOne(options => options.Slot)
            .HasForeignKey<SlotOptions>(options => options.SlotId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Slot>().Property(slot => slot.Settings).HasDefaultValueSql("'{}'");

        modelBuilder.Entity<Realm>().HasOne(realm => realm.RegionSelectionPreference)
            .WithOne(regionSelectionPreference => regionSelectionPreference.Realm).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RealmRegionSelectionPreference>()
            .Property(preference => preference.RegionSelectionPreference)
            .HasDefaultValue(RegionSelectionPreference.AutomaticOwner);
    }
}
