using Microsoft.EntityFrameworkCore;
using Minecraft_Realms_Emulator.Entities;

namespace Minecraft_Realms_Emulator.Data
{
    public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
    {
        public DbSet<World> Worlds { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Backup> Backups { get; set; }
        public DbSet<Invite> Invites { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Configuration> Configuration { get; set; }
        public DbSet<Slot> Slots { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<SeenNotification> SeenNotifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<World>().OwnsOne(w => w.RegionSelectionPreference, b => b.ToJson());

            modelBuilder.Entity<Slot>()
                .HasOne(s => s.World)
                .WithMany(w => w.Slots);
        }
    }
}
