using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Core.Data;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<AppConfig> AppConfig { get; set; }
}
