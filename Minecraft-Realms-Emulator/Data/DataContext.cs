using Microsoft.EntityFrameworkCore;
using Minecraft_Realms_Emulator.Entities;

namespace Minecraft_Realms_Emulator.Data
{
    public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
    {
        public DbSet<World> Worlds { get; set; }
    }
}
