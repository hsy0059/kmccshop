using Microsoft.EntityFrameworkCore;
using Campus.Service.Models.Entities;

namespace Campus.Service.Data;

public class CampusDbContext : Campus.Infrastructure.BaseDbContext
{
    public CampusDbContext(DbContextOptions<CampusDbContext> options) : base(options) { }

    public DbSet<School> Schools => Set<School>();
    public DbSet<CampusEntity> Campuses => Set<CampusEntity>();
    public DbSet<DeliveryZone> DeliveryZones => Set<DeliveryZone>();
}