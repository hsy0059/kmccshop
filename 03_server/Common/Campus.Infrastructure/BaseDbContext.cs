using System.ComponentModel.DataAnnotations.Schema;
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
        ApplySnakeCaseNaming(modelBuilder);
    }

    /// <summary>
    /// 自动将表名和列名转换为 snake_case，与数据库 schema 保持一致。
    /// 已有 [Column]/[Table] 特性的属性不会被覆盖。
    /// </summary>
    private static void ApplySnakeCaseNaming(ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            var tableName = entity.GetTableName();
            if (tableName != null && !HasConventionAttribute(entity.ClrType))
            {
                entity.SetTableName(ToSnakeCase(tableName));
            }

            foreach (var property in entity.GetProperties())
            {
                var propertyInfo = property.PropertyInfo;
                if (propertyInfo != null && !propertyInfo.GetCustomAttributes(typeof(ColumnAttribute), false).Any())
                {
                    property.SetColumnName(ToSnakeCase(property.GetColumnName() ?? property.Name));
                }
            }
        }
    }

    private static bool HasConventionAttribute(Type type)
    {
        return type.GetCustomAttributes(typeof(TableAttribute), false).Any();
    }

    private static string ToSnakeCase(string name)
    {
        return string.Concat(name.Select((c, i) =>
            i > 0 && char.IsUpper(c) ? "_" + char.ToLowerInvariant(c) : char.ToLowerInvariant(c).ToString()));
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
