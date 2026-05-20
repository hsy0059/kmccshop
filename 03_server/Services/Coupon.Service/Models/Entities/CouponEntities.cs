using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coupon.Service.Models.Entities;

[Table("coupon")]
public class Coupon
{
    [Key] public long Id { get; set; }
    [MaxLength(100)] public string Name { get; set; } = string.Empty;
    [MaxLength(255)] public string? Description { get; set; }
    public int Type { get; set; }
    public decimal DiscountValue { get; set; }
    public decimal MinAmount { get; set; }
    public decimal? MaxDiscount { get; set; }
    public int TotalCount { get; set; }
    public int ReceivedCount { get; set; }
    public int UsedCount { get; set; }
    public int PerUserLimit { get; set; } = 1;
    public long? MerchantId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int? ValidDays { get; set; }
    public int Status { get; set; } = 1;
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}

[Table("user_coupon")]
public class UserCoupon
{
    [Key] public long Id { get; set; }
    public long UserId { get; set; }
    public long CouponId { get; set; }
    public int Status { get; set; } = 1;
    public long? OrderId { get; set; }
    [Column("received_at")]
    public DateTime ReceivedAt { get; set; } = DateTime.Now;
    [Column("used_at")]
    public DateTime? UsedAt { get; set; }
    [Column("expire_at")]
    public DateTime? ExpireAt { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}