using Microsoft.EntityFrameworkCore;
using Delivery.Service.Models.Entities;

namespace Delivery.Service.Data;

public class DeliveryDbContext : Campus.Infrastructure.BaseDbContext
{
    public DeliveryDbContext(DbContextOptions<DeliveryDbContext> options) : base(options) { }

    public DbSet<Rider> Riders => Set<Rider>();
}