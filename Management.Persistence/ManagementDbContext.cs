using Management.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Management.Persistence;

public class ManagementDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<SavedFile> SavedFiles { get; set; }
    public ManagementDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
