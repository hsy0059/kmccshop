using Microsoft.EntityFrameworkCore;
using Merchant.Service.Models.Entities;

namespace Merchant.Service.Data;

public class MerchantDbContext : Campus.Infrastructure.BaseDbContext
{
    public MerchantDbContext(DbContextOptions<MerchantDbContext> options) : base(options) { }

    public DbSet<Models.Entities.Merchant> Merchants => Set<Models.Entities.Merchant>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductSpec> ProductSpecs => Set<ProductSpec>();
}