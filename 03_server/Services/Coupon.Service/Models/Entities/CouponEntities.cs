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
    [Column("discount_value")] public decimal DiscountValue { get; set; }
    [Column("min_amount")] public decimal MinAmount { get; set; }
    [Column("max_discount")] public decimal? MaxDiscount { get; set; }
    [Column("total_count")] public int TotalCount { get; set; }
    [Column("received_count")] public int ReceivedCount { get; set; }
    [Column("used_count")] public int UsedCount { get; set; }
    [Column("per_user_limit")] public int PerUserLimit { get; set; } = 1;
    [Column("merchant_id")] public long? MerchantId { get; set; }
    [Column("start_time")] public DateTime StartTime { get; set; }
    [Column("end_time")] public DateTime EndTime { get; set; }
    [Column("valid_days")] public int? ValidDays { get; set; }
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
    [Column("user_id")] public long UserId { get; set; }
    [Column("coupon_id")] public long CouponId { get; set; }
    public int Status { get; set; } = 1;
    [Column("order_id")] public long? OrderId { get; set; }
    [Column("received_at")]
    public DateTime ReceivedAt { get; set; } = DateTime.Now;
    [Column("used_at")]
    public DateTime? UsedAt { get; set; }
    [Column("expire_at")]
    public DateTime? ExpireAt { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}