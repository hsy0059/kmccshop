using Microsoft.EntityFrameworkCore;
using User.Service.Models.Entities;

namespace User.Service.Data;

public class UserDbContext : Campus.Infrastructure.BaseDbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

    public DbSet<Models.Entities.User> Users => Set<Models.Entities.User>();
    public DbSet<Models.Entities.Role> Roles => Set<Models.Entities.Role>();
    public DbSet<Models.Entities.UserRole> UserRoles => Set<Models.Entities.UserRole>();
    public DbSet<Models.Entities.UserAddress> UserAddresses => Set<Models.Entities.UserAddress>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Models.Entities.User>(entity =>
        {
            entity.HasIndex(e => e.Phone).IsUnique();
            entity.HasIndex(e => e.WechatOpenid).IsUnique();
            entity.HasIndex(e => e.UserType);
        });

        modelBuilder.Entity<Models.Entities.Role>(entity =>
        {
            entity.HasIndex(e => e.Code).IsUnique();
        });

        modelBuilder.Entity<Models.Entities.UserRole>(entity =>
        {
            entity.HasIndex(e => new { e.UserId, e.RoleId }).IsUnique();
        });

        modelBuilder.Entity<Models.Entities.UserAddress>(entity =>
        {
            entity.HasIndex(e => e.UserId);
        });

        modelBuilder.Entity<Models.Entities.Feedback>(entity =>
        {
            entity.HasIndex(e => e.UserId);
        });
    }
}
