using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Campus.Infrastructure;

public class BaseDbContext : DbContext
{
    public BaseDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added))
        {
            if (entry.Properties.Any(p => p.Metadata.Name == "CreatedAt"))
            {
                entry.Property("CreatedAt").CurrentValue = DateTime.Now;
            }
        }

        foreach (var entry in ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified))
        {
            if (entry.Properties.Any(p => p.Metadata.Name == "UpdatedAt"))
            {
                entry.Property("UpdatedAt").CurrentValue = DateTime.Now;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
