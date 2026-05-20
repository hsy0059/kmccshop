using Microsoft.EntityFrameworkCore;
using Coupon.Service.Models.Entities;

namespace Coupon.Service.Data;

public class CouponDbContext : Campus.Infrastructure.BaseDbContext
{
    public CouponDbContext(DbContextOptions<CouponDbContext> options) : base(options) { }

    public DbSet<Models.Entities.Coupon> Coupons => Set<Models.Entities.Coupon>();
    public DbSet<UserCoupon> UserCoupons => Set<UserCoupon>();
}